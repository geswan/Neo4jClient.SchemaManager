namespace Neo4jClient.SchemaManager
{
    using System.Text;

    public enum TypeId
    {
        Constraint,

        Index,

        ConstrainedIndex
    };

    public interface IIndexMetadata
    {
        string Constraint { get; set; }

        string Label { get; set; }

        string Property { get; set; }

        string Status { get; set; }

        TypeId Type { get; set; }

        string ToString();
    }

    public class IndexMetadata : IIndexMetadata
    {
        #region Public Properties

        public string Constraint { get; set; }

        public string Label { get; set; }

        public string Property { get; set; }

        public string Status { get; set; }

        public TypeId Type { get; set; }

        public override string ToString()
        {
            StringBuilder sb= new StringBuilder(this.Type.ToString());
            sb.Append(' ').Append(this.Label).Append('.').Append(this.Property).Append(' ').Append(this.Status);
            return sb.ToString();
        }

        #endregion
    }
}