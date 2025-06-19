using System;

namespace TruckTest.Entities;

public class Job
{
    private int m_id;
    private char m_type;

    public int Id
    {
        get { return m_id; }
        set { m_id = value; }
    }

    public Char Type
    {
        get { return m_type; }
        set { m_type = value; }
    }
}
