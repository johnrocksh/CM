namespace CastManager.Templates
{
    /// <summary>
    /// TemplateType  contains all  template types
    /// </summary>
    public enum TemplatePageNum
    {
        // We do not using template for all Orign page num
        Orign, 
        Orign_SelectedDesktop, 
        Orign_SelectedTablo,   
        Horizontal, // One picture above another
        Vertical,   // One picture next to another
        Grid,       // Four pictures at square
        ImageAdv,   // Advertising picture
    }
}
