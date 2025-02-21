using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProductList", menuName = "Product List")]
public class ProductList : ScriptableObject
{
    public List<Product> products;
}