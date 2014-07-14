using FluentNHibernate.Mapping;

namespace DoneRightPass.Model
{

	/// <summary>
	/// A class to used to describe the type of a record
	/// </summary>
	public class RecordType
	{
		public virtual int Id { get; set; }
		/// <summary>
		/// The name of the record type
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// A description of the type
		/// </summary>
		public virtual string Description { get; set; }
	}

	public class RecordTypeMap : ClassMap<RecordType>
	{
		public RecordTypeMap()
		{
			Id(x => x.Id);
			Map(x => x.Name);
			Map(x => x.Description);
			Table("RecordType");
		}
	}
}