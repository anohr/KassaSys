namespace KassaSys.Campaign;

public class CampaignList
{
    public int Id { get; set; }
    public int ProductID { get; set; }
    public string Discount { get; set; }
    public DateTime StartDate { get; set; }
    public int EndDate { get; set; }
}