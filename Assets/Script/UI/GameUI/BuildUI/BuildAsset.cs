using UnityEngine;
using UnityEngine.UI;

public class BuildAsset : MonoBehaviour
{
    public Image image;
    public Color orinalColor;
    public bool canBuild = false;
    public Button button;
    //�����������Դ���Ӵ���������Դ���������
    public CreateRequiredResource currentRequireResource;
    //ľ��
    public int wood;
    //ʳ��
    public int food;
    //����
    public int iron;
    //ʬ��
    public int corpse;
    //����
    public int power;
    //ˮ
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
    //����������ж��Ƿ�����UI���洴���������������������ļ�����Դ�������Ľű���
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
