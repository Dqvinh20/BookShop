using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookShop.Core.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Product
{
    [JsonProperty("id")]
    public int? Id
    {
    get; set; }

    [JsonProperty("discount")]
    public double Discount
    {
        get; set;
    } = 0;

    [JsonProperty("name")]
    public string Name
    {
        get; set;
    } = string.Empty;

    [JsonProperty("description")]
    public string Description
    {
        get; set;
    } = string.Empty;

    [JsonProperty("author")]
    public string Author
    {
        get; set;
    } = string.Empty;

    [JsonProperty("image")]
    public string Image
    {
        get; set;
    }= string.Empty;

    [JsonProperty("image_path")]
    public string ImagePath
    {
        get; set;
    } = string.Empty;

    [JsonProperty("published_year")]
    public int PublishedYear
    {
        get; set;
    } = (int)DateTime.Now.Year;

    [JsonProperty("publisher")]
    public string Publisher
    {
        get; set;
    } = string.Empty;

    [JsonProperty("price")]
    public int Price
    {
        get; set;
    } = 0;

    [JsonProperty("discount_price")]
    public int DiscountPrice
    {
        get; set;
    } = 0;

    [JsonProperty("org_price")]
    public int OriginalPrice
    {
        get; set;
    } = 0;

    [JsonProperty("quantity")]
    public int Quantity
    {
        get; set;
    } = 0;

    [JsonProperty("org_quantity")]
    public int OriginalQuantity
    {
        get; set;
    } = 0;

    [JsonProperty("created_at")]
    public DateTime? CreatedAt
    {
        get; set;
    }

    [JsonProperty("updated_at")]
    public DateTime? UpdatedAt
    {
        get; set;
    }

    [JsonProperty("category_id")]
    public int CategoryId
    {
        get; set;
    } = -1;

    [JsonProperty("categories")]
    public Categories? Category
    {
        get; set; 
    }

    public bool ValidateField()
    {
        return Image != string.Empty && Name != string.Empty && Author != string.Empty && Publisher != string.Empty && CategoryId != -1;
    }

    public bool IsSameProduct(object obj) => obj is Product product && Name == product.Name && Author == product.Author && PublishedYear == product.PublishedYear && Publisher == product.Publisher && CategoryId == product.CategoryId;

    public override string ToString()
    {
        return $"Product(Id = {Id}, Name = {Name}, Author = {Author}, Publisher = {Publisher}, " +
            $"PublishedYear = {PublishedYear}, Price = {Price}, OrgPrice = {OriginalPrice}, " +
            $"Quantity = {Quantity}, OrgQuantity = {OriginalQuantity}, " +
            $"Discount = {Discount}, CategoryId = {CategoryId}, " +
            $"Category={Category ?? null})";
    }

    public override bool Equals(object obj)
    {
        return IsSameProduct(obj as Product) && this.Category == (obj as Product).Category;
    }
}
