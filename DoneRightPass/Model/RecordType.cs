namespace DoneRightPass.Model
{
	/// <summary>
	///     A class to used to describe the type of a record
	/// </summary>
	public class RecordType
	{
		public virtual int Id { get; set; }

		/// <summary>
		///     The name of the record type
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		///     A description of the type
		/// </summary>
		public virtual string Description { get; set; }
	}
}