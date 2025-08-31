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
            Debug.Log("��ȷ��������������ű�");
            return;
        }
        towerName.text = detailDescribe.towerDescribe.name;
        towerDescribe.text = detailDescribe.towerDescribe.desctibe;
    }
    public void FindTower(GameObject tower)
    {
        this.tower = tower.GetComponentInChildren<Tower>();
    }
    //�����
    public void DismantleTowerUI()
    {
        //����ģ�������⣬����ǰ�ڵĴ�������˴����ģ���ϣ������޸���Ϊ�鷳��ֻ�ܸ���һ���������ж���
        if (tower != null)
        {
            tower.DismantleTower();
        }
        inGameUI.OffDetailUI();
    }
}
