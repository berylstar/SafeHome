using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData item;

	public string GetInteractPrompt()
	{
		return string.Format("Pickup {0}", item.itemName);
	}

	public void OnInteract()
	{
		//������ ��ȣ�ۿ��� �Ͼ�� �������̴� �������� ������Ű�� Ŭ���� ���� ���ӿ�����Ʈ��
		//������ �κ��丮 Ŭ������ AddItem�Լ��� �̿��Ͽ� ���Կ� �߰���Ų��.
		Inventory.instance.AddItem(item);
		Destroy(gameObject);
	}
}
