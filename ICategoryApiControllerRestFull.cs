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
/// Create a translation for the category
/// </summary>
/// <param name="categoryTranslation"></param>
/// <returns></returns>
Task<RequestResultWithData<CategoryTranslation>> CreateTranslationAsync(CategoryTranslation categoryTranslation);
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

/// <summary>
/// List all translation for a specific category
/// </summary>
/// <param name="id"></param>
/// <param name="pageIndex"></param>
/// <param name="pageSize"></param>
/// <returns></returns>
Task<ResultDataList<CategoryTranslation>> ListTranslationAsync(Guid id, int? pageIndex, int pageSize = 20);

/// <summary>
/// Get a specific translation by Id
/// </summary>
/// <param name="id"></param>
/// <param name="categoryId"></param>
/// <returns></returns>
Task<RequestResultWithData<CategoryTranslation>> GetTranslationByIdAsync(Guid categoryId, Guid id);

//Remove a categroy
Task<RequestResult> RemoveAsync(Guid CategoryId);

/// <summary>
/// Remove a translation for a category
/// </summary>
/// <param name="CategoryId"></param>
/// <param name="translationId"></param>
/// <param name="translation"></param>
/// <returns></returns>
Task<RequestResult> RemoveTranslationAsync(Guid CategoryId, Guid translationId);

/// <summary>
/// Update a category
/// </summary>
/// <param name="categoryId"></param>
/// <param name="category"></param>
/// <returns></returns>
Task<RequestResult> UpdateAsync(Guid categoryId, Category category);

/// <summary>
/// Update a translation for a category
/// </summary>
/// <param name="categoryId"></param>
/// <param name="translationId"></param>
/// <param name="translation"></param>
/// <returns></returns>
Task<RequestResult> UpdateTranslationAsync(Guid categoryId, Guid translationId, CategoryTranslation translation);
/// <summary>
/// Search a category
/// </summary>
/// <param name="filterExpression">Linq expression</param>
/// <param name="pageIndex"></param>
/// <param name="pageSize"></param>
/// <param name="culture"></param>
/// <returns></returns>
Task<ResultDataList<CategoryDetailViewModel>> SearchAsync(string filterExpression, int? pageIndex, int pageSize = 20, string culture = "fr");

/// <summary>
/// Set the category SEO
/// </summary>
/// <param name="categoryId"></param>
/// <param name="seo"></param>
/// <returns></returns>
Task<RequestResult> SetSEOAsync(Guid categoryId, SEOCategoryRequest seo);
