using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class ItemSlot
{
	public ItemData item;
	public int quantity;
}
public class Inventory : MonoBehaviour
{
	public ItemSlotUI[] uiSlot;
	public ItemSlot[] slots;

	public GameObject inventoryWindow;
	public Transform dropPosition;

	[Header("Selected Item")]
	private ItemSlot selectedItem;
	private int selectedItemIndex;
	public TextMeshProUGUI selectedItemName;
	public TextMeshProUGUI selectedItemDescription;
	public TextMeshProUGUI selectedItemStatNames;
	public TextMeshProUGUI selectedItemStatValues;
	public GameObject useButton;
	public GameObject equipButton;
	public GameObject unEquipButton;
	public GameObject dropButton;

	public ItemData pickaxe; //������ GameManager���� �����;� �� �κ�
	private int curEquipIndex;
	private PlayerController controller;
	[Header("Events")]
	public UnityEvent onOpenInventory;
	public UnityEvent onCloseInventory;

	public static Inventory instance;


	// Start is called before the first frame update
	private void Awake()
	{
		instance = this;
		controller = GetComponent<PlayerController>();
		//����� �����;ߵ�
	}

	private void Start()
	{	
		inventoryWindow.SetActive(false);
		slots = new ItemSlot[uiSlot.Length];

		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = new ItemSlot();
			uiSlot[i].index = i;
			uiSlot[i].Clear();

		}
		ClearSelectedItemWindow();
		AddItem(pickaxe); //������ GameManager���� �����;� �� �κ�
	}

	public void Toggle()
	{
		if (inventoryWindow.activeInHierarchy)
		{
			inventoryWindow.SetActive(false);
			onCloseInventory?.Invoke();
			controller.ToggleCursor(false);
		}
		else
		{
			inventoryWindow.SetActive(true);
			onOpenInventory?.Invoke();
			controller.ToggleCursor(true);
		}
	}

	public void OnInventoryButton(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.phase == InputActionPhase.Started)
		{
			Toggle();
		}
	}

	public bool IsOpen()
	{
		return inventoryWindow.activeInHierarchy;
	}

	public void AddItem(ItemData item) 
	{
		if (item.canStack)
		{
			ItemSlot slotToStakTo = GetItemStack(item);
			if (slotToStakTo != null)
			{
				slotToStakTo.quantity++;
				UpdateUI();
				return;
			}
		}

		ItemSlot emptySlot = GetEmptySlot();

		if (emptySlot != null)
		{
			emptySlot.item = item;
			emptySlot.quantity = 1;
			UpdateUI();
			return;
		}
	}

	void UpdateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item != null)
				uiSlot[i].Set(slots[i]);
			else
				uiSlot[i].Clear();
		}
	}

	ItemSlot GetItemStack(ItemData item) //������ ã�ƿ��� �Լ�
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)
				return slots[i];
		}
		return null;
	}

	ItemSlot GetEmptySlot() //�������� ����ִٸ�
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item == null)
				return slots[i];
		}
		return null;
	}

	public void SelectItem(int index)
	{
		if (slots[index].item == null)
			return;

		selectedItem = slots[index];
		selectedItemIndex = index;

		selectedItemName.text = selectedItem.item.itemName;
		selectedItemDescription.text = selectedItem.item.description;

		//selectedItemStatNames.text = string.Empty;
		//selectedItemStatValues.text = string.Empty;

		//for (int i = 0; i < selectedItem.item.consumables.Length; i++)
		//{
		//	selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n";
		//	selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
		//}

		useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
		equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlot[index].equipped);
		unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlot[index].equipped);
		dropButton.SetActive(selectedItem.item.type != ItemType.Equipable);
	}

	private void ClearSelectedItemWindow()
	{
		selectedItem = null;
		selectedItemName.text = string.Empty;
		selectedItemDescription.text = string.Empty;

		//selectedItemStatNames.text = string.Empty;
		//selectedItemStatValues.text = string.Empty;  //���� ���� �������

		equipButton.SetActive(false);
		dropButton.SetActive(false);
		useButton.SetActive(false);
		unEquipButton.SetActive(false);
	}

	//public void OnUseButton()
	//{
	//	if (selectedItem.item.type == ItemType.Consumable)
	//	{
	//		for (int i = 0; i < selectedItem.item.consumables.Length; i++)
	//		{
	//			switch (selectedItem.item.consumables[i].type)
	//			{
	//				case ConsumableType.Health:
	//					condition.Heal(selectedItem.item.consumables[i].value); break;
	//				case ConsumableType.Hunger:
	//					condition.Eat(selectedItem.item.consumables[i].value); break;
	//			}
	//		}
	//	}
	//	RemoveSelectedItem();
	//}

	public void OnEquipButton()
	{
		if (uiSlot[curEquipIndex].equipped)
		{
			UnEquip(curEquipIndex);
		}

		uiSlot[selectedItemIndex].equipped = true;
		curEquipIndex = selectedItemIndex;
		EquipManager.instance.EquipNew(selectedItem.item);
		UpdateUI();

		SelectItem(selectedItemIndex);
	}

	void UnEquip(int index)
	{
		uiSlot[index].equipped = false;
		EquipManager.instance.UnEquip();
		UpdateUI();

		if(selectedItemIndex == index)
		{
			SelectItem(index);
		}
	}

	public void OnUnEquipButton()
	{
		UnEquip(selectedItemIndex);
	}
	public void OnDropButton()
	{
		RemoveSelectedItem();
	}

	private void RemoveSelectedItem()
	{
		selectedItem.quantity--;

		if (selectedItem.quantity <= 0)
		{
			if (uiSlot[selectedItemIndex].equipped)
			{
				UnEquip(selectedItemIndex);
			}

			selectedItem.item = null;
			ClearSelectedItemWindow();
		}
		UpdateUI();
	}

	public void RemoveItem(ItemData item)
	{

	}

	public bool HasItems(ItemData item, int quantity)
	{
		return false;
	}
}