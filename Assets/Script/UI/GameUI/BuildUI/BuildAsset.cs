using UnityEngine;
using UnityEngine.UI;

public class BuildAsset : MonoBehaviour
{
    public Image image;
    public Color orinalColor;
    public bool canBuild = false;
    public Button button;
    //创建所需的资源，从创建所需资源管理器获得
    public CreateRequiredResource currentRequireResource;
    //木材
    public int wood;
    //食物
    public int food;
    //金属
    public int iron;
    //尸块
    public int corpse;
    //电力
    public int power;
    //水
    public int water;
    public virtual void Awake()
    {
        image = GetComponent<Image>();
        orinalColor = image.color;
        button = GetComponent<Button>();
    }
    public virtual void Update()
    {
        if(button == null)
        {
            return;
        }
        if (canBuild)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public virtual bool CanBuildTower()
    {
        if (wood <= SourceManager.instance.GetSourceData("wood") && food <= SourceManager.instance.GetSourceData("food") && iron <= SourceManager.instance.GetSourceData("iron") && corpse <= SourceManager.instance.GetSourceData("corpse") && power <= SourceManager.instance.GetSourceData("power") && water <= SourceManager.instance.GetSourceData("water"))
        {
            return true;
        }
        return false;
    }
    public void SetColorGray()
    {
        image.color = Color.gray;
    }
    public void SetColorOrinal()
    {
        image.color = orinalColor;
    }
    //这个是用来判断是否能在UI界面创建塔，至于塔创建后在哪减少资源，在塔的脚本里
    public virtual void SetSource(CreateRequiredResource currentRequireResource)
    {
        this.wood = currentRequireResource.wood;
        this.food = currentRequireResource.food;
        this.iron = currentRequireResource.iron;
        this.corpse = currentRequireResource.corpse;
        this.power = currentRequireResource.power;
        this.water = currentRequireResource.water;
    }
    public virtual bool GetCanBuild() => canBuild;
    public virtual void SetCanBuild(bool canbuild) => canBuild = canbuild;
}
