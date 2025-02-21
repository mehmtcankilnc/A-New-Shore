using UnityEngine;

[CreateAssetMenu(fileName = "NewProduct", menuName = "Product")]
public class Product : ScriptableObject
{
    public string productName;
    public int price;
    public int maxQuantity;
}