using UnityEngine;

public interface IPickUpable 
{
    public PickUpMode dropMode { get; set; }

    public void PickUp(Transform holdPoint);
}

public enum PickUpMode
{
    None,
    Drop,
    Throw
}
