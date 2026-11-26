namespace BrightMinds.Core.Enums
{
    /// <summary>
    /// Selects the unit level in which a raw-material recipe ingredient quantity is entered.
    /// Nullable on recipe ingredients to preserve legacy recipes that only use UnitType.
    /// </summary>
    public enum RawMaterialUnitLevel
    {
        Purchase = 1,
        Middle = 2,
        Smallest = 3
    }
}
