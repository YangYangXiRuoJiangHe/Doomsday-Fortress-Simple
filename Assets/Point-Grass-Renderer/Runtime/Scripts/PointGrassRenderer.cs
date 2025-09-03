using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MicahW.PointGrass {
    using static DistributePointsAlongMesh;
    using static PointGrassCommon;

    [ExecuteAlways]
    public class PointGrassRenderer : MonoBehaviour {
        [Header("Distribution Parameters")]
        //草点生成的位置来源，是网格，地形还是其他物体
        public DistributionSource distSource = DistributionSource.Mesh;
        //如果为mesh，则使用此和表面采样顶点
        public Mesh baseMesh = default;
        //地形组件数据，
        public TerrainData terrain;
        //可根据纹理混合决定草的密度
        public TerrainLayer[] terrainLayers;
        //将渲染区域划分为多少块，可用于LOD或分块剔除
        public Vector2Int chunkCount = new Vector2Int(8, 8);
        //可选从场景中的其他meshFilter获取几何信息
        public MeshFilter[] sceneFilters;
        private MeshFilter filter;

        [Header("Grass Parameters")]
        //flat使用双面着色的Quad平面模拟草，mesh使用网格模型
        public BladeType bladeType = BladeType.Flat;
        //选择是否多个不同草模型进行混合渲染
        public bool multipleMeshes = false;
        private bool UsingMultipleMeshes => bladeType == BladeType.Mesh && multipleMeshes;
        public Mesh grassBladeMesh = default;
        public Mesh[] grassBladeMeshes = new Mesh[1];
        //每个草模型的相对密度权重
        public float[] meshDensityValues = new float[] { 1f };
        //选择是否多个不同材质，可为每个mesh选单独材质，或统一用一个材质
        public bool multipleMaterials = false;
        public Material material = default;
        public Material[] materials = new Material[1];
        //阴影设置，启用，不起用，两面渲染，仅渲染阴影
        public UnityEngine.Rendering.ShadowCastingMode shadowMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //渲染层级
        public SingleLayer renderLayer;

        [Header("Point Parameters")]
        //生成多少个草点
        public float pointCount = 1000f;
        //若启用，则不使用生成草点，改为每单位面积数量，false则总共生成pointCount，true为每平方米生成pointCount
        public bool multiplyByArea = false;
        //动态调整LOD，选择渲染多少草，用于性能测试
        [Range(0f, 1f)] public float pointLODFactor = 1f;
        //随机草种子，控制位置，旋转等伪随机性
        public bool randomiseSeed = true;
        //固定种子可复现结果
        public int seed = 0;
        //是否强制所有草的法线方向，否则根据表面法线调整
        public bool overwriteNormalDirection;
        public Vector3 forcedNormal = Vector3.up;
        //基于噪声或纹理密度图进行剔除（低于阈值不生成草）
        public bool useDensity = true;
        [Range(0f, 1f)] public float densityCutoff = 0.5f;
        //映射草的高度范围
        public bool useLength = true;
        public Vector2 lengthMapping = new Vector2(0f, 1f);

        // 单网格模式下的点数据缓冲
        //存储每个草实例的数据（位置，旋转，缩放，颜色）
        private ComputeBuffer pointBuffer;
        //传递材质参数（颜色，风动画等）
        private MaterialPropertyBlock materialBlock;
        //用于GPU剔除和光照计算
        private Bounds boundingBox;
        //多网格模式下的点数据缓冲（草，材质，包围盒）
        private ComputeBuffer[] pointBuffers;
        private MaterialPropertyBlock[] materialBlocks;
        private Bounds[] boundingBoxes;
        //如何将草投射到地面上
        [Header("Projection Parameters")]
        //None不投影，生成在挂载的物体mesh上，ProjectMesh投影到物体下方的网格表面上
        public ProjectionType projectType;
        //只投影到哪些层
        public LayerMask projectMask = ~0;

        [Header("Bounding Box Parameters")]
        //调整渲染包围盒，影响视锥剔除范围，可防止边缘闪烁
        public Bounds boundingBoxOffset = new Bounds(Vector3.zero, Vector3.one);


//仅在编译器中编译和执行
#if UNITY_EDITOR
        //unity的回调方法，选中物体时可视化调试信息
        private void OnDrawGizmosSelected() {
            //获取本地包围盒，将其转换到世界空间中然后应用偏移
            Bounds renderBounds = AddBoundsExtrusion(TransformBounds(GetLocalBounds()));
            //白色线框立方体绘制表示草的渲染范围
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(renderBounds.center, renderBounds.size);
            //如果分布元是mesh且basemesh（plane最好）不为空，则绘制basemesh线框
            if (distSource == DistributionSource.Mesh && baseMesh != null) {
                Gizmos.color = Color.cyan;
                //设置 Gizmos 的变换矩阵，使后续绘制的线框相对于当前 GameObject 的 Transform 进行变换。
                Gizmos.matrix = transform.localToWorldMatrix;
                //绘制 baseMesh 的线框。
                Gizmos.DrawWireMesh(baseMesh);
                //将 Gizmos 的变换矩阵重置为单位矩阵，避免影响后续绘制。
                Gizmos.matrix = Matrix4x4.identity;
            }
            //如果分布源是 TerrainData 且 terrain 不为空，则绘制一些辅助线。
            if (distSource == DistributionSource.TerrainData && terrain != null) {
                Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
                Gizmos.matrix = transform.localToWorldMatrix;
                //获取地形的尺寸。
                Vector3 size = terrain.size;
                //绘制代表分块（chunks）边界的线。这些线位于地形高度的 1/4 处，以避免遮挡。
                for (int x = 1; x < chunkCount.x; x++) { Gizmos.DrawLine(new Vector3((float)x / chunkCount.x * size.x, size.y * 0.25f, 0f), new Vector3((float)x / chunkCount.x * size.x, size.y * 0.25f, size.z)); }
                for (int y = 1; y < chunkCount.y; y++) { Gizmos.DrawLine(new Vector3(0f, size.y * 0.25f, (float)y / chunkCount.y * size.z), new Vector3(size.x, size.y * 0.25f, (float)y / chunkCount.y * size.z)); }
                Gizmos.matrix = Matrix4x4.identity;
                //如果存在分块包围盒数组，则遍历并绘制每个分块的包围盒（带偏移），颜色为淡青色。
                if (boundingBoxes != null) {
                    Gizmos.color = new Color(0.5f, 1f, 1f, 1f);
                    for (int i = 0; i < boundingBoxes.Length; i++) {
                        Bounds bound = AddBoundsExtrusion(TransformBounds(boundingBoxes[i]));
                        Gizmos.DrawWireCube(bound.center, bound.size);
                    }
                }
            }
        }
        //当脚本在inspector值变化时Unity会调用此方法,通常用于自动校验并同步数据
        private void OnValidate() {
            //如果选择多个草模型
            if (UsingMultipleMeshes && grassBladeMeshes != null) {
                // 获取草模型数组的长度。
                int grassMeshCount = grassBladeMeshes.Length;
                // 如果 meshDensityValues 数组为空，则创建一个新数组，长度与 grassBladeMeshes 相同，并将所有元素初始化为 1f。
                if (meshDensityValues == null) {
                    meshDensityValues = new float[grassMeshCount];
                    for (int i = 0; i < grassMeshCount; i++) { meshDensityValues[i] = 1f; }
                }
                else if (meshDensityValues.Length != grassMeshCount) {
                    //如果 meshDensityValues 数组存在但长度不匹配，则使用 System.Array.Resize 将其调整到正确的长度。
                    System.Array.Resize(ref meshDensityValues, grassMeshCount);
                }
                // 如果 materials 数组为空，则创建一个新数组，长度与 grassBladeMeshes 相同。
                if (materials == null) { materials = new Material[grassMeshCount]; }
                else if (multipleMaterials && materials.Length != grassMeshCount) {
                    //如果 multipleMaterials 为 true 但 materials 数组长度不匹配，则将其调整到正确长度。
                    System.Array.Resize(ref materials, grassMeshCount);
                }
            }
        }
        //不在编译器中
#endif
        //Unity 的回调方法。当在 Inspector 中点击脚本组件的齿轮图标并选择 "Reset" 时，或当脚本首次被添加到 GameObject 上时，Unity 会调用此方法。用于将脚本的所有公共字段重置为其默认状态。
        private void Reset() {
            //调用 ClearBuffers 方法，释放可能存在的 GPU 缓冲区。
            ClearBuffers();

            bladeType = BladeType.Flat;
            multipleMeshes = false;

            grassBladeMesh = null;
            grassBladeMeshes = new Mesh[1];
            meshDensityValues = new float[] { 1f };
            multipleMaterials = false;

            material = null;
            materials = new Material[1];
            shadowMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderLayer.Set(gameObject.layer);

            pointCount = 1000f;
            multiplyByArea = false;
            pointLODFactor = 1f;

            randomiseSeed = true;
            seed = 0;

            overwriteNormalDirection = false;
            forcedNormal = Vector3.up;

            useDensity = true;
            densityCutoff = 0.5f;
            useLength = true;

            projectType = ProjectionType.None;
            projectMask = ~0;
        }

        private void OnEnable() {
            // 进行兼容性检查。如果失败，则直接返回，不再执行后续代码。
            if (!CompatibilityCheck()) return;
            // 检查是否已经初始化了材质属性 ID（PropertyIDsInitialized 和 FindPropertyIDs 应在 PointGrassCommon 中定义）。如果没有，则调用 FindPropertyIDs 来查找并缓存这些 ID，以提高后续 SetBuffer 等操作的性能。
            if (!PropertyIDsInitialized) { FindPropertyIDs(); }
            // 检查用于 Flat 和 Cylindrical 类型的默认草模型网格（位于PointGrassCommon中）是否已生成。如果未生成，则调用 GenerateGrassMeshes() 方法创建它们。
            if (grassMeshFlat == null || grassMeshCyl == null) { GenerateGrassMeshes(); }
            //最后，调用 BuildPoints 方法，开始生成草点数据。
            BuildPoints();
        }
        //释放所有已分配的 GPU 计算缓冲区，防止内存泄漏。
        private void OnDisable() { ClearBuffers(); }
        //Unity 的回调方法。在 Update 方法执行之后、所有动画更新之后、渲染之前调用。
        //调用 DrawGrass 方法来渲染草。
        // TODO : Point Grass Renderer - Look into implementing the drawing routine as a custom renderer so the grass will still be drawn while the game is paused in the editor
        private void LateUpdate() { DrawGrass(); }

        /// <summary>Checks if the system supports the necessary features used by the point grass</summary>
        /// <returns><c>bool</c> - Equals <c>true</c> if the system supports all the required features</returns>
        //检查当前运行系统的硬件和图形 API 是否支持此脚本所需的关键功能
        private bool CompatibilityCheck() {
            // Helper function with preprocessor directives to disable/destroy this component
            //删除该组件
            void Disable() {
#if UNITY_EDITOR
                //在编辑器中，只是将 enabled 设置为 false。
                enabled = false;
#else
                //在发布版中，调用 Destroy(this) 销毁组件。
                Destroy(this);
#endif
            }
            //检查系统是否支持 GPU Instancing。如果不支持，则报错并调用 Disable()。因为这是此脚本的核心技术，用于高效渲染大量相同的草模型。
            // If the system doesn't support instancing, write to the console and destroy this component
            if (!SystemInfo.supportsInstancing) {
                Debug.LogError($"This system doesn't support instanced draw calls. \"{gameObject.name}\" is unable to render its point grass");
                Disable();
                return false;
            }
            //检查系统的图形着色器级别是否低于 4.5。ComputeBuffer 和 DrawMeshInstancedProcedural 等高级功能需要 Shader Model 4.5 或更高。如果不满足，则报错并调用 Disable()。
            // If the system's shader level is too low, write to the console and destroy this component
            else if (SystemInfo.graphicsShaderLevel < 45) {
                Debug.LogError($"This system doesn't support shader model 4.5. Compute buffers are unsupported in the point grass shaders");
                Disable();
                return false;
            }

            return true;
        }
        //用于重新生成草点数据。通常在参数改变或 OnEnable 时调用。
        /// <summary>Clears any existing buffers and builds new ones</summary>
        public void BuildPoints() {
            //首先清理旧的缓冲区。
            // Clear Buffers
            ClearBuffers();
            //根据 randomiseSeed 的值决定使用随机种子还是固定种子。
            // Get the seed
            int seed = randomiseSeed ? Random.Range(int.MinValue, int.MaxValue) : this.seed;
            // 创建一个可空的 Vector3 变量来存储强制法线方向。
            Vector3? overwriteNormal = null;
            //如果启用了强制法线
            if (overwriteNormalDirection) {
                // 将 forcedNormal 归一化。
                forcedNormal = forcedNormal.normalized;
                //将其从世界空间转换到本地空间（使用 InverseTransformDirection），因为后续的分布计算通常在本地空间进行。
                overwriteNormal = transform.InverseTransformDirection(forcedNormal);
            }
            // 如果分布源是地形数据且分块数大于 1，则调用 BuildPoints_Terrain。
            if (distSource == DistributionSource.TerrainData && chunkCount.x > 1 && chunkCount.y > 1) { BuildPoints_Terrain(seed, overwriteNormal); }
            else {
                //否则，调用 BuildPoints_Mesh。这涵盖了 Mesh, MeshFilter, SceneFilters 以及不分块的 TerrainData。
                BuildPoints_Mesh(seed, overwriteNormal); 
            }
        }
        //用于处理非分块地形的草点生成。
        /// <summary>Builds the point buffers using mesh data</summary>
        /// <param name="seed">The random/preset seed</param>
        private void BuildPoints_Mesh(int seed, Vector3? overwriteNormal) {
            // 调用 GetMeshData 方法获取用于分布的几何数据。如果获取失败，则返回。
            if (!GetMeshData(out MeshData mesh)) { return; }
            //  如果启用了投影，则调用 ProjectBaseMesh 方法，将 mesh 的顶点投影到 projectMask 指定的表面上。
            if (projectType == ProjectionType.ProjectMesh) { ProjectBaseMesh(ref mesh, projectMask, transform); }
            //记录投影后网格的包围盒。
            boundingBox = mesh.bounds;

            // 调用 DistributePoints 函数（来自 DistributePointsAlongMesh 静态类）来实际生成草点数组。该函数会根据网格的顶点、三角形、密度图、长度图、种子、数量等参数，生成一个包含位置、旋转、缩放、颜色等信息的 MeshPoint 数组。
            MeshPoint[] points = DistributePoints(mesh, transform.localScale, pointCount, seed, multiplyByArea, overwriteNormal, true, useDensity, useLength, densityCutoff, lengthMapping);
            // 如果成功生成了草点，则调用 CreateBuffers 方法创建 GPU 缓冲区。
            if (points != null && points.Length > 0) { CreateBuffers(points); }
        }
        //用于处理分块地形的草点生成。
        /// <summary>Builds the point buffers using terrain data</summary>
        /// <param name="seed">The random/preset seed</param>
        private void BuildPoints_Terrain(int seed, Vector3? overwriteNormal) {
            // 使用 List<T> 来动态收集每个分块生成的数据。
            List<ComputeBuffer> bufferList = new List<ComputeBuffer>();
            List<MaterialPropertyBlock> blockList = new List<MaterialPropertyBlock>();
            List<Bounds> boundsList = new List<Bounds>();
            // 分块的大小数量
            Vector3 chunkSize = terrain.size;
            chunkSize.x /= chunkCount.x;
            chunkSize.z /= chunkCount.y;
            // 调用 CacheTerrainData 函数（来自 DistributePointsAlongMesh）来缓存地形数据，为后续的网格生成做准备。
            CacheTerrainData(terrain, terrainLayers);
            //遍历 chunkCount.x 和 chunkCount.y 指定的所有分块。
            for (int x = 0; x < chunkCount.x; x++) {
                for (int y = 0; y < chunkCount.y; y++) {
                    // 调用 GetTerrainMeshData 获取当前分块 (x, y) 的网格数据。
                    bool validMesh = GetTerrainMeshData(out MeshData data, new Vector2Int(x, y));
                    //没有该分块数据，直接继续下一个分块
                    if (!validMesh) { continue; }
                    // 确保不同分块的草点分布是独立的。
                    int newSeed = seed + x + y * chunkCount.x;
                    // 在当前分块的网格数据上分布草点。
                    MeshPoint[] points = DistributePoints(data, transform.localScale, pointCount, newSeed, multiplyByArea, overwriteNormal, true, useDensity, useLength, -1f, lengthMapping);
                    //没有草点继续下一个分块。
                    if (points == null || points.Length == 0) { continue; }

                    // 检查是否使用多个草模型。
                    if (UsingMultipleMeshes) {
                        // 根据 meshDensityValues 将 points 数组分割成多个 ComputeBuffer 和 MaterialPropertyBlock。
                        CreateBuffersFromPoints_Multi(points, out ComputeBuffer[] buffers, out MaterialPropertyBlock[] blocks);
                        // 将分割后的缓冲区和材质块添加到列表中。
                        if (buffers == null) { continue; }
                        bufferList.AddRange(buffers);
                        blockList.AddRange(blocks);
                    }
                    else {
                        // 创建单个缓冲区
                        CreateBufferFromPoints(points, out ComputeBuffer buffer, out MaterialPropertyBlock block);
                        bufferList.Add(buffer);
                        blockList.Add(block);
                    }
                    // 为当前分块内的所有草点计算一个包围盒，并添加到 boundsList。
                    Bounds bounds = new Bounds(points[0].position, Vector3.zero);
                    for (int i = 1; i < points.Length; i++) { bounds.Encapsulate(points[i].position); }
                    boundsList.Add(bounds);
                }
            }
            // 如果成功生成了任何数据，则将 List<T> 转换为数组并赋值给类的字段 pointBuffers, materialBlocks, boundingBoxes。
            if (bufferList.Count > 0) {
                //草点缓冲
                pointBuffers = bufferList.ToArray();
                //材质缓冲
                materialBlocks = blockList.ToArray();
                //区块缓冲
                boundingBoxes = boundsList.ToArray();
            }
        }
        /// <summary>Retrieves/creates mesh data based on the renderer's parameters</summary>
        /// <param name="meshData">The output mesh data</param>
        /// <returns><c>bool</c> - Equals true if the output mesh data is valid</returns>
        //根据 distSource 的值，从不同的来源获取或创建 MeshData。
        private bool GetMeshData(out MeshData meshData) {
            //初始化输出参数。
            meshData = MeshData.Empty;
            //根据 distSource 的值进行分支。
            switch (distSource) {
                //分支一
                case DistributionSource.Mesh:
                    //baseMesh 为 null，则使用 goto case 跳转到 MeshFilter 分支。
                    if (baseMesh == null) { goto case DistributionSource.MeshFilter; }
                    // 用于存储每个顶点的附加属性（密度和长度）。
                    Vector2[] meshAttributes = new Vector2[baseMesh.vertexCount];
                    //如果 baseMesh 有颜色且 useDensity 或 useLength 为 true，则将颜色的 R 通道作为密度，G 通道作为长度，存储在 meshAttributes 中。
                    if (baseMesh.colors != null && baseMesh.colors.Length == baseMesh.vertexCount && (useDensity || useLength)) {
                        for (int i = 0; i < meshAttributes.Length; i++) { meshAttributes[i] = new Vector2(baseMesh.colors[i].r, baseMesh.colors[i].g); }
                    }
                    //将所有 meshAttributes 设置为 Vector2.one（表示密度和长度均为 1.0）。
                    else { for (int i = 0; i < meshAttributes.Length; i++) { meshAttributes[i] = Vector2.one; } }
                    //使用 baseMesh 的顶点、法线、UV、三角形和 meshAttributes 创建 MeshData 对象。
                    meshData = new MeshData(baseMesh.vertices, baseMesh.normals, baseMesh.uv, baseMesh.triangles, meshAttributes);
                    return true;

                case DistributionSource.MeshFilter:
                    //获取本 GameObject 上的 MeshFilter 组件。
                    filter = GetComponent<MeshFilter>();
                    if (filter != null) {
                        baseMesh = filter.sharedMesh;
                        if (baseMesh != null) {
                            // 如果存在且其 sharedMesh 不为空，则与 Mesh 分支类似，创建 MeshData。
                            Vector2[] filterAttributes = new Vector2[baseMesh.vertexCount];
                            if (baseMesh.colors != null && baseMesh.colors.Length == baseMesh.vertexCount && (useDensity || useLength)) {
                                for (int i = 0; i < filterAttributes.Length; i++) { filterAttributes[i] = new Vector2(baseMesh.colors[i].r, baseMesh.colors[i].g); }
                            }
                            else { for (int i = 0; i < filterAttributes.Length; i++) { filterAttributes[i] = Vector2.one; } }
                            // Create the mesh data
                            meshData = new MeshData(baseMesh.vertices, baseMesh.normals, baseMesh.uv, baseMesh.triangles, filterAttributes);
                            return true;
                        }
                    }
                    break;
                //分支二
                case DistributionSource.TerrainData:
                    //调用 CacheTerrainData。PointGrassCommon中
                    CacheTerrainData(terrain, terrainLayers);
                    //调用 GetTerrainMeshData 获取整个地形的网格数据（Vector2Int.zero 表示不进行分块）。
                    return GetTerrainMeshData(out meshData, Vector2Int.zero);

                case DistributionSource.SceneFilters:
                    //如果 sceneFilters 数组不为空，则调用 CreateMeshFromFilters 函数（来自 DistributePointsAlongMesh），将场景中指定的多个 MeshFilter 的网格合并成一个大的 MeshData。
                    if (sceneFilters != null && sceneFilters.Length > 0) {
                        //PointGrassCommon中
                        meshData = CreateMeshFromFilters(transform, sceneFilters);
                        //确保定点参数不为0
                        if (meshData.verts.Length > 0) { return true; }
                    }
                    break;
            }
            //如果所有分支都未能成功获取数据，则返回 false
            return false;
        }
        /// <summary>Gets a generated mesh from terrain data. Requires the terrain data to be cached first with 'CacheTerrainData()'</summary>
        /// <param name="meshData">The output mesh data</param>
        /// <param name="chunkCoord">The coordinate of the chunk</param>
        /// <returns><c>bool</c> - Equals <c>true</c> if the mesh was successfully generated</returns>
        //用于从 TerrainData 生成指定分块的 MeshData。
        private bool GetTerrainMeshData(out MeshData meshData, Vector2Int chunkCoord) {
            meshData = MeshData.Empty;

            if (terrain != null && terrainLayers != null) {
                // 获取地形高度图的分辨率。
                int terrainSize = terrain.heightmapResolution;
                //计算当前分块 (chunkCoord) 在高度图上的起始坐标。
                int startX = Mathf.FloorToInt((float)terrainSize * chunkCoord.x / chunkCount.x);
                int startY = Mathf.FloorToInt((float)terrainSize * chunkCoord.y / chunkCount.y);
                //计算当前分块 (chunkCoord) 在高度图上的结束坐标。
                int endX = Mathf.CeilToInt((float)terrainSize * (chunkCoord.x + 1) / chunkCount.x);
                int endY = Mathf.CeilToInt((float)terrainSize * (chunkCoord.y + 1) / chunkCount.y);
                //根据 useDensity 决定是否应用密度剔除。
                float denCutoff = useDensity ? densityCutoff : 0f;
                //位于pointGrassCommon
                meshData = CreateMeshFromTerrainData(terrain, denCutoff, startX, startY, endX - startX, endY - startY);

                return true;
            }

            return false;
        }

        /// <summary>Creates the renderer's buffers from an array of type <c>MeshPoint</c></summary>
        /// <param name="points">The array of points used to create the buffers</param>
        //根据 UsingMultipleMeshes 的值，选择调用 CreateBuffersFromPoints_Multi 或 CreateBufferFromPoints 来创建缓冲区。
        private void CreateBuffers(MeshPoint[] points) {
            if (UsingMultipleMeshes) { CreateBuffersFromPoints_Multi(points, out pointBuffers, out materialBlocks); }
            else { CreateBufferFromPoints(points, out pointBuffer, out materialBlock); }
        }
        /// <summary>Clears all the buffers and arrays from the renderer</summary>
        //用于释放所有已分配的 ComputeBuffer 并将数组引用设置为 null，防止内存泄漏。
        private void ClearBuffers() {
            if (pointBuffer != null) { pointBuffer.Release(); }
            if (pointBuffers != null) {
                for (int i = 0; i < pointBuffers.Length; i++) { pointBuffers[i].Release(); }
                pointBuffers = null;
            }
            if (boundingBoxes != null) { boundingBoxes = null; }
        }

        /// <summary>Creates a compute buffer and material property block from an array of type <c>MeshPoint</c></summary>
        /// <param name="points">The array of points</param>
        /// <param name="buffers">The output compute buffer</param>
        /// <param name="blocks">The output material property block</param>
        //用于为一个 MeshPoint 数组创建单个 ComputeBuffer 和 MaterialPropertyBlock
        private void CreateBufferFromPoints(MeshPoint[] points, out ComputeBuffer buffer, out MaterialPropertyBlock block) {
            // 检查输入数据的有效性。
            if (points == null || points.Length == 0) { buffer = null; block = null; return; }
            // 创建一个新的 ComputeBuffer。points.Length 是元素数量，MeshPoint.stride 是每个 MeshPoint 结构体在内存中的字节大小（由 PointGrassCommon 定义）。
            buffer = new ComputeBuffer(points.Length, MeshPoint.stride);
            // 将 MeshPoint 数组的数据复制到 GPU 缓冲区中。
            buffer.SetData(points);
            // 调用 CreateMaterialPropertyBlock 方法创建材质属性块。
            block = CreateMaterialPropertyBlock(buffer);
        }
        /// <summary>Creates multiple buffers and material property blocks from an array of type <c>MeshPoint</c></summary>
        /// <param name="points">The array of points</param>
        /// <param name="buffers">The output compute buffers</param>
        /// <param name="blocks">The output material property blocks</param>
        //用于将一个 MeshPoint 数组根据 meshDensityValues 分割成多个 ComputeBuffer 和 MaterialPropertyBlock。
        private void CreateBuffersFromPoints_Multi(MeshPoint[] points, out ComputeBuffer[] buffers, out MaterialPropertyBlock[] blocks) {
            if (points == null || points.Length == 0) { buffers = null; blocks = null; return; }
            // Local copies of the array lengths for readability
            int pointCount = points.Length;
            int meshCount = grassBladeMeshes.Length;
            // 如果草点总数少于草模型数量，则无法为每个模型分配至少一个点，直接返回 null。
            if (pointCount < meshCount) { buffers = null; blocks = null; } // Since some buffers would have a size of 0, return nothing to prevent errors
            else {
                // Normalize the density values of each mesh by dividing it by the sum
                float sum = 0f;
                float[] densities = new float[meshCount];
                //计算所有密度权重的总和。
                for (int i = 0; i < meshCount; i++) { sum += meshDensityValues[i]; }
                if (sum <= 0) {
                    // 如果总和为 0 或负数（无效），则将每个模型的密度设置为 1f / meshCount（平均分配）。
                    float val = 1f / meshCount;
                    for (int i = 0; i < meshCount; i++) { densities[i] = val; }
                }
                else for (int i = 0; i < meshCount; i++) {
                        //将每个模型的密度权重归一化（除以总和），得到一个概率分布。
                        densities[i] = meshDensityValues[i] / sum; } // Sum is greater than 0. Use a divide

                // Create the buffer and block arrays
                //创建meshCount个缓冲和区块数组
                buffers = new ComputeBuffer[meshCount];
                blocks = new MaterialPropertyBlock[meshCount];

                // Fill the buffers
                int dataPointer = 0;
                //遍历每个草模型。
                for (int i = 0; i < meshCount; i++) {
                    //计算该模型应该分配到的草点数量（目标值）。
                    int targetCount = Mathf.RoundToInt(pointCount * densities[i]);
                    //计算剩余的草点数和剩余的缓冲区数量。
                    int remainingPoints = pointCount - dataPointer;
                    int remainingBuffers = (meshCount - i) - 1;
                    //确保每个缓冲区至少有 1 个点，在分配时，为后面的缓冲区预留足够的点
                    // Calculate the size of this buffer so it uses at least 1 point and leaves 1 points for the remaining buffers
                    int bufferSize = Mathf.Max(1, Mathf.Min(remainingPoints - (remainingBuffers), targetCount)); // The final size of the buffer
                    //为每一个缓冲区创建指定大小的数量为bufferSize的缓冲区。
                    buffers[i] = new ComputeBuffer(bufferSize, MeshPoint.stride); // Create the buffer
                    //points 数组的 dataPointer 位置开始，复制 bufferSize 个元素到新创建的缓冲区中。
                    buffers[i].SetData(points, dataPointer, 0, bufferSize); // Fill the buffer with the next set of points
                    //为这个缓冲区创建对应的材质属性块。
                    blocks[i] = CreateMaterialPropertyBlock(buffers[i]); // Create a material property block
                    //更新数据指针，指向下一个要分配的草点。
                    dataPointer += bufferSize; // Increase the data pointer
                }
            }
        }
        //负责最终的渲染调用。
        /// <summary>Draws the grass</summary>
        private void DrawGrass() {
            // 检查是否有有效的缓冲区可以渲染。
            if (pointBuffer == null && pointBuffers == null) { return; }
            //获取当前要渲染的草模型网格（根据 bladeType 返回 grassMeshFlat, grassMeshCyl, grassBladeMesh）。
            Mesh mesh = GetGrassMesh();
            //获取当前要使用的材质。
            Material mat = material;
            //获取并转换最终的渲染包围盒。
            Bounds finalBounds = TransformBounds(GetLocalBounds());
            //计算一些布尔值，用于简化后续的条件判断，存在草点的缓冲且缓冲数量不为0。
            bool hasPointBuffers = pointBuffers != null && pointBuffers.Length > 0;
            //使用多种草网格模型并且草模型不为null
            bool useMultipleMeshes = UsingMultipleMeshes && grassBladeMeshes != null && hasPointBuffers;
            //使用单种草网格模型并且材质数组不为null
            bool useMultipleMats = multipleMaterials && materials != null && hasPointBuffers;

            //  如果存在分块包围盒（即 BuildPoints_Terrain 被调用过）。
            if (boundingBoxes != null && boundingBoxes.Length > 0) {
                if (hasPointBuffers) {
                    // 计算每个分块对应的缓冲区数量。
                    int buffersPerBounds = pointBuffers.Length / boundingBoxes.Length;
                    // Update the booleans for multiple meshes and materials
                    useMultipleMeshes &= grassBladeMeshes.Length >= buffersPerBounds;
                    useMultipleMats &= materials.Length >= buffersPerBounds;

                    // 外层遍历每个分块
                    for (int i = 0; i < boundingBoxes.Length; i++) {
                        finalBounds = TransformBounds(boundingBoxes[i]);
                        // 内层遍历该分块内的每个缓冲区。
                        for (int j = 0; j < buffersPerBounds; j++) {
                            int index = i * buffersPerBounds + j;
                            // 如果使用多个网格，则根据内层循环索引 j 选择当前要渲染的 mesh 。
                            if (useMultipleMeshes) {
                                mesh = grassBladeMeshes[j];
                                // 如果使用多个材质，则根据内层循环索引 j 选择当前要渲染的 material
                                if (useMultipleMats) { mat = materials[j]; }
                            }
                            //调用 DrawGrassBuffer 渲染当前缓冲区。
                            DrawGrassBuffer(pointBuffers[index], ref materialBlocks[index], mesh, mat, finalBounds);
                        }
                    }
                }
                // Backup for scene filters
                else { DrawGrassBuffer(pointBuffer, ref materialBlock, mesh, mat, TransformBounds(boundingBoxes[0])); }
            }
            // 如果没有分块包围盒但存在 pointBuffers 数组（即 BuildPoints_Mesh 被调用且 UsingMultipleMeshes 为 true）。
            else if (hasPointBuffers) {
                // Update the booleans for multiple meshes and materials
                useMultipleMeshes &= grassBladeMeshes.Length >= pointBuffers.Length;
                useMultipleMats &= materials.Length >= pointBuffers.Length;
                // 遍历所有缓冲区
                for (int i = 0; i < pointBuffers.Length; i++) {
                    if (useMultipleMeshes) {
                        //根据索引 i 选择 mesh 和 mat。
                        mesh = grassBladeMeshes[i];
                        if (useMultipleMats) { mat = materials[i]; }
                    }
                    //调用 DrawGrassBuffer 渲染当前缓冲区。
                    DrawGrassBuffer(pointBuffers[i], ref materialBlocks[i], mesh, mat, finalBounds);
                }
            }
            // 使用单一的 pointBuffer 和 materialBlock 进行渲染。
            else { DrawGrassBuffer(pointBuffer, ref materialBlock, mesh, mat, finalBounds); }
            
        }
        /// <summary>Draws a single grass buffer</summary>
        /// <param name="buffer">The buffer of type <c>MeshPoint</c> used for drawing</param>
        /// <param name="block">A reference to the buffer's corresponding <c>MaterialPropertyBlock</c></param>
        /// <param name="mesh">The mesh to be drawn for each <c>MeshPoint</c></param>
        /// <param name="mat">The material used for rendering</param>
        /// <param name="bounds">The bounding box</param>
        private void DrawGrassBuffer(ComputeBuffer buffer, ref MaterialPropertyBlock block, Mesh mesh, Material mat, Bounds bounds) {
            if (buffer == null || !buffer.IsValid()) { return; }
            //根据 LOD 因子（如相机距离）动态减少草的数量。
            int count = Mathf.CeilToInt(buffer.count * pointLODFactor);
            // If the mesh isn't null, the material isn't null, and the material property block isn't null, the point count is greater than 0, draw the buffer
            if (mesh != null && mat != null && block != null && count > 0) {
                // 扩大包围盒，防止边缘草被裁剪掉。
                bounds = AddBoundsExtrusion(bounds);
                // 设置 Shader 所需的变换矩阵等参数
                UpdateMaterialPropertyBlock(ref block, transform);
                // 从 GPU buffer 中读取数据，绘制 count 个实例。true 表示接收阴影。renderLayer.LayerIndex 控制渲染层。最后参数为 null，表示不使用额外的剔除遮罩。
                Graphics.DrawMeshInstancedProcedural(mesh, 0, mat, bounds, count, block, shadowMode, true, renderLayer.LayerIndex, null);
            }

        }
        /// <summary> Creates a <c>MaterialPropertyBlock</c> used with rendering the grass meshes </summary>
        /// <param name="pointBuffer">The compute buffer supplied to the material property block</param>
        /// <returns><c>MaterialPropertyBlock</c> - A new material property block with the supplied compute buffer</returns>
        //创建并初始化一个 MaterialPropertyBlock，用于在 Shader 中访问草点数据。
        private MaterialPropertyBlock CreateMaterialPropertyBlock(ComputeBuffer pointBuffer) {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            // 将 pointBuffer 绑定到 Shader 中名为 ID_PointBuff 的 Compute Buffer 变量：
            block.SetBuffer(ID_PointBuff, pointBuffer);
            //设置 Shader 所需的变换矩阵等参数
            UpdateMaterialPropertyBlock(ref block, transform);
            // Return the property block
            return block;
        }

        /// <summary>Retrieves a blade mesh depending on the renderer's blade type</summary>
        /// <returns><c>Mesh</c> - The blade mesh</returns>
        private Mesh GetGrassMesh() {
            switch (bladeType) {
                case BladeType.Flat:        return grassMeshFlat;
                case BladeType.Cylindrical: return grassMeshCyl;
                case BladeType.Mesh:        return grassBladeMesh;
                default:                    throw new System.ArgumentException(message: "Invalid enum value");
            };
        }
        /// <summary>Gets the local bounding box based on the distribution source and renderer state</summary>
        /// <returns><c>Bounds</c> - The bounding box in local space</returns>
        public Bounds GetLocalBounds() {
            switch (distSource) {
                case DistributionSource.Mesh:
                    return boundingBox;

                case DistributionSource.MeshFilter:
                    goto case DistributionSource.Mesh;

                case DistributionSource.TerrainData:
                    if (terrain == null) { goto default; }
                    //基于地形尺寸创建包围盒（中心在中心，大小等于地形大小）。
                    else { return new Bounds(terrain.size * 0.5f, terrain.size); }

                case DistributionSource.SceneFilters:
                    return boundingBox;

                default:
                    return new Bounds(Vector3.zero, Vector3.one);
            }
        }
        /// <summary>Transforms the bounding box based on the bounding box's corners</summary>
        /// <param name="localBounds">The bounding box in local space</param>
        /// <returns><c>Bounds</c> - The bounding box transformed from the renderer's local space to world space</returns>
       //将本地空间的包围盒转换为世界空间的包围盒（考虑物体的旋转、缩放、位移）。
        private Bounds TransformBounds(Bounds localBounds) {
            // Get the min and max points
            Vector3 min = localBounds.min;
            Vector3 max = localBounds.max;
            // 获取包围盒的 8 个角点。
            Vector3[] points = new Vector3[] {
                min, max,

                new Vector3(max.x, min.y, min.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(min.x, min.y, max.z),

                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(max.x, max.y, min.z),
            };
            // 使用 transform.TransformPoint() 将每个角点转为世界坐标。
            for (int i = 0; i < points.Length; i++) { points[i] = transform.TransformPoint(points[i]); }
            // 重新构建一个新的包围盒
            localBounds = new Bounds(points[0], Vector3.zero);
            //之所以不能直接 Transform，是因为旋转/缩放会使 AABB（轴对齐包围盒）不再对齐，必须重新计算最小包围盒。
            for (int i = 1; i < points.Length; i++) { localBounds.Encapsulate(points[i]); }
            // Return the bounding box
            return localBounds;
        }
        /// <summary>Applys <c>boundingBoxOffset</c> to the input bounding box</summary>
        /// <param name="worldSpaceBounds">The input bounding box</param>
        /// <returns><c>Bounds</c> - The extruded bounding box</returns>
        //对世界空间的包围盒进行偏移和扩展，防止草叶在边缘被裁剪
        private Bounds AddBoundsExtrusion(Bounds worldSpaceBounds) {
            //中心加上 boundingBoxOffset.center
            worldSpaceBounds.center += boundingBoxOffset.center;
            //大小加上 boundingBoxOffset.size
            worldSpaceBounds.size += boundingBoxOffset.size;
            return worldSpaceBounds;
        }


        // TODO : Point Grass Renderer - Double-check these set functions
        #region Parameter Update Methods
        public void SetDistributionSource(Mesh mesh) {
            if (mesh == null) { Debug.LogError($"An attempt was made to set the distribution source on \"{gameObject.name}\" to null. Make sure the input distribution source is not null"); return; }
            distSource = DistributionSource.Mesh; baseMesh = mesh; // Set the distribution source and base mesh
        }
        public void SetDistributionSource(MeshFilter filter) {
            if (filter == null) { Debug.LogError($"An attempt was made to set the distribution source on \"{gameObject.name}\" to null. Make sure the input distribution source is not null"); return; }

            if (filter.gameObject == gameObject) { distSource = DistributionSource.MeshFilter; } // If the filter is on this object, then we just need to set the distribution source (since the MeshFilter distribution source doesn't keep a reference to the filter)
            else { SetDistributionSource(new MeshFilter[] { filter }); } // Since the filter isn't on this object, we need to use the scene filters distribution source
        }
        public void SetDistributionSource(TerrainData terrain) {
            if (terrain == null) { Debug.LogError($"An attempt was made to set the distribution source on \"{gameObject.name}\" to null. Make sure the input distribution source is not null"); return; }
            distSource = DistributionSource.TerrainData; this.terrain = terrain; // Set the distribution source and terrain data
        }
        public void SetDistributionSource(MeshFilter[] sceneFilters) {
            if (sceneFilters == null) { Debug.LogError($"An attempt was made to set the distribution source on \"{gameObject.name}\" to null. Make sure the input distribution source is not null"); return; }
            distSource = DistributionSource.SceneFilters; this.sceneFilters = sceneFilters; // Set the distribution source and scene filter references
        }

        public void SetBladeType(BladeType type) { bladeType = type; }
        public void SetBladeMesh(Mesh mesh) {
            if (mesh == null) { Debug.LogError($"An attempt was made to set the blade mesh on \"{gameObject.name}\" to null or an empty array. Make sure the input blade mesh is not null"); return; }
            // Set the blade mesh. And make sure multiple meshes is disabled
            SetBladeType(BladeType.Mesh);
            grassBladeMesh = mesh;
            multipleMeshes = false;
        }
        public void SetBladeMesh(Mesh[] meshes, float[] meshDensityValues = null, Material[] materials = null) {
            // Check if the meshes are valid
            if (meshes == null || meshes.Length == 0) { Debug.LogError($"An attempt was made to set the blade meshes on \"{gameObject.name}\" to null or an empty array. Make sure the input blade meshes are not null"); return; }

            if (meshes.Length == 1) { SetBladeMesh(meshes[0]); } // If there's only one mesh in the array, use the normal mesh blade mode instead
            else {
                // Set the blade type, meshes and enable multiple meshes
                SetBladeType(BladeType.Mesh);
                grassBladeMeshes = meshes;
                multipleMeshes = true;
                // Set mesh density values, resizing if necessary
                if (meshDensityValues != null && meshDensityValues.Length > 0) { this.meshDensityValues = meshDensityValues; }
                if (this.meshDensityValues.Length != meshes.Length) { System.Array.Resize(ref this.meshDensityValues, meshes.Length); }
                // Set materials
                if (materials != null && materials.Length > 0) { SetMaterials(materials); }
            }
        }
        public void SetBladeDensities(float[] densities) {
            if (densities == null || densities.Length == 0) { Debug.LogError($"An attempt was made to set the blade densities on \"{gameObject.name}\" to null or an empty array. Make sure the input blade densities are not null"); return; }

            if (densities.Length != grassBladeMeshes.Length) { System.Array.Resize(ref densities, grassBladeMeshes.Length); }
            meshDensityValues = densities;
        }
        public void SetMaterial(Material mat) {
            if (mat == null) { Debug.LogError($"An attempt was made to set the blade material on \"{gameObject.name}\" to null or an empty array. Make sure the input blade material is not null"); return; }

            multipleMaterials = false;
            material = mat;
        }
        public void SetMaterials(Material[] materials) {
            if (materials == null || materials.Length == 0) { Debug.LogError($"An attempt was made to set the blade materials on \"{gameObject.name}\" to null or an empty array. Make sure the input blade materials are not null"); return; }

            if (materials.Length == 1) { SetMaterial(materials[0]); }
            else {
                multipleMaterials = true;
                if (materials.Length != grassBladeMeshes.Length) { System.Array.Resize(ref materials, grassBladeMeshes.Length); }
                this.materials = materials;
            }

        }

        public void SetShadowMode(UnityEngine.Rendering.ShadowCastingMode mode) { shadowMode = mode; }
        public void SetRenderLayer(int layer) { renderLayer.Set(layer); }
        public void SetRenderLayer(SingleLayer layer) { renderLayer = layer; }

        public void SetPointCount(float count, bool multiplyByArea = false) {
            pointCount = count;
            this.multiplyByArea = multiplyByArea;
        }
        public void SetPointLODFactor(float value) { pointLODFactor = Mathf.Clamp01(value); }

        public void SetSeed(int seed) { randomiseSeed = false; this.seed = seed; }
        public void SetSeed(bool randomise) { randomiseSeed = randomise; }

        public void SetOverwriteNormal(Vector3 normal) {
            overwriteNormalDirection = true;
            forcedNormal = normal.normalized;
        }
        public void SetOverwriteNormal(bool enabled) { overwriteNormalDirection = enabled; }

        public void SetDensity(bool enabled, float cutoff = 0.5f) {
            useDensity = enabled;
            densityCutoff = cutoff;
        }
        public void SetLength(bool enabled, float rangeMin = 0.25f, float rangeMax = 1f) {
            useLength = enabled;
            lengthMapping = new Vector2(rangeMin, rangeMax);
        }

        public void SetProjection(ProjectionType type, LayerMask mask) {
            projectType = type;
            projectMask = mask;
        }

        public void SetBoundingBoxOffset(Bounds bounds) { boundingBoxOffset = bounds; }
        #endregion


        /// <summary>Gets debug info about the renderer (Point count, buffer sizes, etc.)</summary>
        /// <returns><c>DebugInformation</c> - The debug information</returns>
        //返回当前渲染器的状态信息，用于调试性能或排查问题。
        public DebugInformation GetDebugInfo() {
            if (!enabled) {
                return new DebugInformation() {
                    totalPointCount = 0,
                    usingMultipleBuffers = false,
                    bufferCount = 0,
                    smallestBuffer = 0,
                    largestBuffer = 0
                };
            }

            DebugInformation info = new DebugInformation();
            info.usingMultipleBuffers = pointBuffers != null;
            if (info.usingMultipleBuffers) {
                int smallest = int.MaxValue;
                int largest = int.MinValue;
                for (int i = 0; i < pointBuffers.Length; i++) {
                    if (pointBuffers[i] != null && pointBuffers[i].IsValid()) {
                        int count = pointBuffers[i].count;
                        info.totalPointCount += pointBuffers[i].count;
                        if (count < smallest) { smallest = count; }
                        if (count > largest) { largest = count; }
                    }
                }
                info.bufferCount = pointBuffers.Length;
                info.smallestBuffer = smallest;
                info.largestBuffer = largest;
            }
            else if (pointBuffer != null && pointBuffer.IsValid()) {
                info.totalPointCount = pointBuffer.count;
                info.bufferCount = 1;
                info.smallestBuffer = info.totalPointCount;
                info.largestBuffer = info.totalPointCount;
            }

            return info;
        }
        /// <summary>A struct used for containing debug information about point grass renderers</summary>
        public struct DebugInformation {
            public int totalPointCount;
            public bool usingMultipleBuffers;
            public int bufferCount;
            public int smallestBuffer;
            public int largestBuffer;
        }
    }
}