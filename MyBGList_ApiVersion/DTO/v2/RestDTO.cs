namespace MyBGList_ApiVersion.DTO;

public class RestDTO<T>
{
	public List<DTO.v1.LinkDTO> Links { get; set; } = new List<DTO.v1.LinkDTO>();

	public T Items { get; set; } = default;
}
