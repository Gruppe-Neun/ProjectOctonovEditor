using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBehavior : MonoBehaviour, IInteractable
{

    public enum ContainerType {
        Tiny,
        Medium,
        Large,
        Olli
    }

    public ContainerType type = ContainerType.Tiny;
    public string containerName = "Container";
    public ItemBehavior[] content;

    // Start is called before the first frame update
    void Start()
    {
        switch (type) {
            case ContainerType.Tiny:
                content = new ItemBehavior[4];
                break;

            case ContainerType.Medium:
                content = new ItemBehavior[8];
                break;

            case ContainerType.Large:
                content = new ItemBehavior[12];
                break;

            case ContainerType.Olli:
                content = new ItemBehavior[5];
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact() {
        //GameObject.Find("UI").GetComponent<UIBehavior>().openContainer(this);
    }

    public virtual void updateContainer() {

    }
}
