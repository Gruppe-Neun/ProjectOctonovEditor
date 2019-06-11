using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlliOrdnerBehavior : ContainerBehavior,IInteractable {
    public GameObject arm_left;
    public GameObject arm_right;
    public GameObject leg_left;
    public GameObject leg_right;
    public GameObject body;


    private bool[] partSlot = new bool[] { false, false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        this.type = ContainerType.Olli;
        this.content = new ItemBehavior[5];
        this.name = "3D Drucker";
        //updateBody();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void updateBody() {
        arm_left.SetActive(partSlot[0]);
        arm_right.SetActive(partSlot[1]);
        leg_left.SetActive(partSlot[2]);
        leg_right.SetActive(partSlot[3]);
        body.SetActive(partSlot[4]);
    } 

    public void addPart(Item.Type part) {
        switch (part) {
            case Item.Type.Olli_ArmLeft:
                partSlot[0] = true;
                break;

            case Item.Type.Olli_ArmRight:
                partSlot[1] = true;
                break;

            case Item.Type.Olli_LegLeft:
                partSlot[2] = true;
                break;

            case Item.Type.Olli_LegRight:
                partSlot[3] = true;
                break;

            case Item.Type.Olli_Body:
                partSlot[4] = true;
                break;

        }
        updateBody();
    }

    public override void updateContainer() {
        base.updateContainer();
        partSlot[0] = (content[0] != null && content[0].type == Item.Type.Olli_ArmLeft);
        partSlot[1] = (content[1] != null && content[1].type == Item.Type.Olli_ArmRight);
        partSlot[2] = (content[2] != null && content[2].type == Item.Type.Olli_LegLeft);
        partSlot[3] = (content[3] != null && content[3].type == Item.Type.Olli_LegRight);
        partSlot[4] = (content[4] != null && content[4].type == Item.Type.Olli_Body);
        updateBody();
    }
}
