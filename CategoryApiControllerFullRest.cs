/// <inheritdoc />
[Produces("application/json")]
[Route("api/category")]
[ValidateModel]
public class CategoryApiController : Controller, ICategoryApiController
{
    private readonly IRepository<Category> _repository;
    private readonly IRepository<CategoryTranslation> _repositoryTranslation;

    public CategoryApiController(IRepository<Category> repository, IRepository<CategoryTranslation> repositoryTranslation)
    {
        _repository = repository;
        _repositoryTranslation = repositoryTranslation;
    }

    /// <inheritdoc />
    [HttpPost("")]
    public async Task<RequestResultWithData<Category>> AddAsync(Category Category)
    {
        if (ModelState.IsValid)
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
        else
        {
            var errorList = (from item in ModelState
                                where item.Value.Errors.Any()
                                select item.Value.Errors[0].ErrorMessage).ToList();

            return new RequestResultWithData<Category>(false, JsonConvert.SerializeObject(errorList));
        }
    }

    /// <inheritdoc />
    [HttpPost("translation")]
    public async Task<RequestResultWithData<CategoryTranslation>> CreateTranslationAsync(CategoryTranslation categoryTranslation)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var creationResult = await _repositoryTranslation.AddAsync(categoryTranslation);
                return new RequestResultWithData<CategoryTranslation>(creationResult.Item1, creationResult.Item2);
            }
            catch (Exception ex)
            {
                return new RequestResultWithData<CategoryTranslation>(false, ex.ToString());
            }
        }
        else
        {
            var errorList = (from item in ModelState
                                where item.Value.Errors.Any()
                                select item.Value.Errors[0].ErrorMessage).ToList();

            return new RequestResultWithData<CategoryTranslation>(false, JsonConvert.SerializeObject(errorList));
        }
    }

    /// <inheritdoc />
    [HttpGet("{id}")]
    public async Task<RequestResultWithData<CategoryDetailViewModel>> GetByIdAsync([FromRoute]Guid id, string culture = "fr")
    {
        if (id == Guid.Empty)
            return new RequestResultWithData<CategoryDetailViewModel>(false, "Id is required");

        var category = await _repository.GetByIdAsync(id);

        var translations = await _repositoryTranslation.SearchAsync(x => x.Name == category.Name && x.Language == culture, 0, 20);

        var categoryDetail = new CategoryDetailViewModel(category);

        if (!translations.Any())
        {
            return new RequestResultWithData<CategoryDetailViewModel>(false, "NotFound");
        }

        categoryDetail.SetTranslation(translations.FirstOrDefault());

        return new RequestResultWithData<CategoryDetailViewModel>(true, categoryDetail);
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

        return new ResultDataList<CategoryDetailViewModel>(false, 0, 0, 0, 0, null);
    }

    [HttpGet("{id}/translation")]
    public async Task<ResultDataList<CategoryTranslation>> ListTranslationAsync([FromRoute]Guid id, int? pageIndex, int pageSize = 20)
    {
        var all = await _repositoryTranslation.ListAsync(pageIndex, pageSize);

        var page = all;

        if (pageIndex.HasValue && pageIndex.GetValueOrDefault(0) > 0)
            page = page.Skip(pageIndex.GetValueOrDefault(0) * pageSize).ToList();

        if (pageSize > 0)
            page = page.Take(pageSize).ToList();

        return new ResultDataList<CategoryTranslation>(true, page.Count, all.Count, pageSize, pageIndex.GetValueOrDefault(0), page);
    }

    [HttpGet("{categoryId}/translation/{id}")]
    public async Task<RequestResultWithData<CategoryTranslation>> GetTranslationByIdAsync([FromRoute]Guid categoryId, [FromRoute]Guid id)
    {
        if (categoryId == Guid.Empty)
            return null;

        if (id == Guid.Empty)
            return null;

        var translation = await _repositoryTranslation.GetByIdAsync(id);

        return new RequestResultWithData<CategoryTranslation>(translation != null, translation);
    }

    [HttpDelete("{id}")]
    public async Task<RequestResult> RemoveAsync([FromRoute]Guid id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category == null)
            return new RequestResult(false, "NotFound");

        var deletedResult = await _repository.DeleteAsync(category);

        return deletedResult ? new RequestResult(true, null) : new RequestResult(false, "can not delete category");
    }

    [HttpDelete("{categoryId}/translation/{translationId}")]
    public async Task<RequestResult> RemoveTranslationAsync([FromRoute]Guid categoryId, [FromRoute]Guid translationId)
    {
        var current = await _repositoryTranslation.GetByIdAsync(translationId);

        if (current == null)
            return new RequestResult(false, "NOTFOUND");

        var deletedResult = await _repositoryTranslation.DeleteAsync(current);

        return new RequestResult(deletedResult, deletedResult ? "" : "ERROR");
    }

    [HttpPut("{categoryId}")]
    public async Task<RequestResult> UpdateAsync([FromRoute]Guid categoryId, Category category)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var updateResult = await _repository.UpdateAsync(category);

                return new RequestResult(updateResult, !updateResult ? "ERROR" : "");
            }
            catch (Exception ex)
            {
                return new RequestResult(false, ex.ToString());
            }
        }
        else
        {
            var errorList = (from item in ModelState
                                where item.Value.Errors.Any()
                                select item.Value.Errors[0].ErrorMessage).ToList();

            return new RequestResult(false, JsonConvert.SerializeObject(errorList));
        }
    }

    [HttpPut("{categoryId}/translation/{translationId}")]
    public async Task<RequestResult> UpdateTranslationAsync([FromRoute]Guid categoryId, [FromRoute]Guid translationId, CategoryTranslation translation)
    {
        if (ModelState.IsValid)
        {
            var result = await _repositoryTranslation.UpdateAsync(translation);

            return new RequestResult(result, !result ? "ERROR" : "");
        }
        else
        {
            var errorList = (from item in ModelState
                                where item.Value.Errors.Any()
                                select item.Value.Errors[0].ErrorMessage).ToList();

            return new RequestResultWithData<Category>(false, JsonConvert.SerializeObject(errorList));
        }
    }

    [HttpGet("search")]
    public async Task<ResultDataList<CategoryDetailViewModel>> SearchAsync(string filterExpression, int? pageIndex, int pageSize = 20, string culture = "fr")
    {
        if (string.IsNullOrEmpty(filterExpression)) return new ResultDataList<CategoryDetailViewModel>(false);

        var lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda<Category, bool>(filterExpression);
        var func = lambda.Compile();

        var data = await _repository.SearchAsync(func, pageIndex, pageSize);
        var translations = await _repositoryTranslation.AllAsync();

        if (data.Any())
        {
            var result = data.Select((c) =>
            {
                var detail = new CategoryDetailViewModel(c);
                detail.SetTranslation(translations.FirstOrDefault(x => x.Name == c.Name && x.Language == culture));
                return detail;
            }).ToList();

            return PagineData(result, pageIndex, pageSize);
        }

        return new ResultDataList<CategoryDetailViewModel>(false);
    }

    [HttpPatch("{categoryId}/seo")]
    public async Task<RequestResult> SetSEOAsync(Guid categoryId, SEOCategoryRequest seo)
    {
        var translations = await _repositoryTranslation.SearchAsync(x => x.CategoryId == seo.CategoryId && x.Language == seo.CultureTwoLetterIso, 0, 20);

        if (translations.Any())
        {
            var translation = translations.FirstOrDefault();

            translation.MetaDescription = seo.MetaDescription;
            translation.MetaKeyWords = seo.MetaKeyWords;
            translation.MetaTitle = seo.MetaTitle;

            return await UpdateTranslationAsync(seo.CategoryId, translation.Id, translation);
        }
        else
        {
            return new RequestResult(false, "NOTFOUND");
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

}
