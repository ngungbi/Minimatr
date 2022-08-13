namespace Minimatr.SampleProject.Models;

public class TestModel {
    /// <summary>
    /// Test integer 32
    /// </summary>
    /// <example>1234</example>

    public string? String { get; set; }
    public bool Bool { get; set; }

    public DateTime DateTime { get; set; }

    public DateOnly DateOnly { get; set; }
    public TimeOnly TimeOnly { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public float Float { get; set; }
    public double Double { get; set; }
    public decimal Decimal { get; set; }
    public short Short { get; set; }
    public int Int32 { get; set; }
    public long Long { get; set; }
    public ushort UShort { get; set; }
    public uint UInt { get; set; }
    public ulong Ulong { get; set; }
    public sbyte SByte { get; set; }
    public byte Byte { get; set; }
    public Guid Guid { get; set; }
    public byte[]? Bytes { get; set; }
}
