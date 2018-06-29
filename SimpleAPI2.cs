/// <inheritdoc />
[Produces("application/json")]
[Route("api/category")]
[ValidateModel]
public class CategoryApiSimpleController : Controller, CMS.WebApi.Controllers.ICategoryApiController
{
private readonly IRepository<Category> _repository;
private readonly IRepository<CategoryTranslation> _repositoryTranslation;

public CategoryApiSimpleController(IRepository<Category> repository, IRepository<CategoryTranslation> repositoryTranslation)
{
    _repository = repository;
    _repositoryTranslation = repositoryTranslation;
}

[HttpGet("")]
public async Task<ResultDataList<CategoryDetailViewModel>> ListAsync(int? pageIndex, int pageSize = 20, string culture = "fr")
{
    var categories = await _repository.ListAsync(pageIndex, pageSize);
    var translations = await _repositoryTranslation.AllAsync();

    if (categories.Any())
    {
        var result = categories.Select((c) =>
        {
            var detail = new CategoryDetailViewModel(c);
            detail.SetTranslation(translations.FirstOrDefault(x => x.Name == c.Name && x.Language == culture));
            return detail;
        }).ToList();

        return PagineData(result, pageIndex, pageSize);
    }

    return new ResultDataList<CategoryDetailViewModel>(false, "NO CATEGORY FOUND");
}

/// <inheritdoc />
[HttpGet("{id}")]
public async Task<RequestResultWithData<CategoryDetailViewModel>> GetByIdAsync([FromRoute]Guid id, string culture = "fr")
{
    if (id == Guid.Empty)
        return new RequestResultWithData<CategoryDetailViewModel>(false, "ID EMPTY");

    var category = await _repository.GetByIdAsync(id);

    var translations = await _repositoryTranslation.SearchAsync(x => x.Name == category.Name && x.Language == culture, 0, 20);

    var categoryDetail = new CategoryDetailViewModel(category);

    if (!translations.Any())
    {
        return new RequestResultWithData<CategoryDetailViewModel>(false, "NO TRANSLATION");
    }

    categoryDetail.SetTranslation(translations.FirstOrDefault());

    return new RequestResultWithData<CategoryDetailViewModel>(true, categoryDetail);
}

/// <inheritdoc />
[HttpPost("")]
public async Task<RequestResultWithData<Category>> AddAsync(Category Category)
{
    try
    {
        var creationResult = await _repository.AddAsync(Category);

        return new RequestResultWithData<Category>(creationResult.Item1, creationResult.Item2);
    }
    catch (Exception ex)
    {
        return new RequestResultWithData<Category>(false, ex.ToString());
    }
}

private ResultDataList<CategoryDetailViewModel> PagineData(List<CategoryDetailViewModel> result, int? pageIndex, int? pageSize)
{
    if (result.Any())
    {
        var page = result;

        if (pageIndex.HasValue)
            page = result.Skip(pageIndex.Value * pageSize.GetValueOrDefault(20)).ToList();

        if (pageSize.HasValue)
            page = page.Take(pageSize.Value).ToList();

        return new ResultDataList<CategoryDetailViewModel>(true, page.Count, result.Count, pageSize.GetValueOrDefault(20), pageIndex.GetValueOrDefault(0), page);
    }
    else
    {
        return new ResultDataList<CategoryDetailViewModel>(false);
    }
}
