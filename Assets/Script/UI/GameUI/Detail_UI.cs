using TMPro;
using UnityEngine;

public class Detail_UI : UI_Response
{
    public TextMeshProUGUI towerName;
    public TextMeshProUGUI towerDescribe;
    public Tower tower;
    public InGame_UI inGameUI;
    public void UpdateDescribeText(DetailDescribe detailDescribe)
    {
        if(detailDescribe == null)
        {
            Debug.Log("请确保你的塔有描述脚本");
            return;
        }
        towerName.text = detailDescribe.towerDescribe.name;
        towerDescribe.text = detailDescribe.towerDescribe.desctibe;
    }
    public void FindTower(GameObject tower)
    {
        this.tower = tower.GetComponentInChildren<Tower>();
    }
    //拆除塔
    public void DismantleTower()
    {
        //这里模型有问题，导致前期的代码放在了错误的模型上，现在修改颇为麻烦，只能给他一个单独的判断了
        if (tower != null)
        {
            if (tower.towerType == TowerType.Tower_MachineGun_Type)
            {
                this.tower.SetFountionEmpty();
                this.tower.ReturnResources(SourceManager.instance.returnSourceMultiplier);
                GameObject tower = this.tower.transform.parent.transform.parent.gameObject;
                Destroy(this.tower.transform.parent.transform.parent.gameObject);
            }
            else
            {
                tower.SetFountionEmpty();
                this.tower.ReturnResources(SourceManager.instance.returnSourceMultiplier);
                Destroy(tower.gameObject);
            }
        }
        inGameUI.OffDetailUI();
    }
}
