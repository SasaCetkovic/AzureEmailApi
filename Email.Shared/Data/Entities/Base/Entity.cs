using System.ComponentModel.DataAnnotations;

namespace Email.Shared.Data.Entities.Base
{
	public abstract class Entity : IEntity
	{
		[Key]
		public long Id { get; set; }
	}
}
