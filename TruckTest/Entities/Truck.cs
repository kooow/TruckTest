namespace TruckTest.Entities;

public class Truck
{
    private char[] m_compatibleJobTypes;
    private int m_id;

    public int Id
    {
        get { return m_id; }
        set { m_id = value; }
    }

    public char[] CompatibleJobTypes
    {
        get { return m_compatibleJobTypes; }
        set { m_compatibleJobTypes = value; }
    }
}
