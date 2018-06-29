/// <summary>
/// Interface implemented by the Category API
/// </summary>
public interface ICategoryApiController
{
    /// <summary>
    /// Add a new category
    /// </summary>
    /// <param name="Category"></param>
    /// <returns></returns>
    Task<RequestResultWithData<Category>> AddAsync(Category Category);

    /// <summary>
    /// Get category
    /// </summary>
    /// <param name="id"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    Task<RequestResultWithData<CategoryDetailViewModel>> GetByIdAsync(Guid id, string culture = "fr");

    /// <summary>
    /// List all category in a specific culture can be pagined
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    Task<ResultDataList<CategoryDetailViewModel>> ListAsync(int? pageIndex, int pageSize = 20, string culture = "fr");
}
