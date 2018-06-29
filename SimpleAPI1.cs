namespace CMS.WebApi.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/category")]
    [ValidateModel]
    public class CategoryApiSimpleController : Controller
    {
        private readonly IRepository<Category> _repository;
        private readonly IRepository<CategoryTranslation> _repositoryTranslation;

        public CategoryApiSimpleController(IRepository<Category> repository, IRepository<CategoryTranslation> repositoryTranslation)
        {
            _repository = repository;
            _repositoryTranslation = repositoryTranslation;
        }

        [HttpGet("")]
        public async Task<IActionResult> ListAsync(int? pageIndex, int pageSize = 20, string culture = "fr")
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

                return new OkObjectResult(PagineData(result, pageIndex, pageSize));
            }

            return new BadRequestResult();
        }

        /// <inheritdoc />
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute]Guid id, string culture = "fr")
        {
            if (id == Guid.Empty)
                return new BadRequestResult();

            var category = await _repository.GetByIdAsync(id);

            var translations = await _repositoryTranslation.SearchAsync(x => x.Name == category.Name && x.Language == culture, 0, 20);

            var categoryDetail = new CategoryDetailViewModel(category);

            if (!translations.Any())
            {
                return new NotFoundResult();
            }

            categoryDetail.SetTranslation(translations.FirstOrDefault());

            return new OkObjectResult(new RequestResultWithData<CategoryDetailViewModel>(true, categoryDetail));
        }

        /// <inheritdoc />
        [HttpPost("")]
        public async Task<IActionResult> AddAsync(Category Category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var creationResult = await _repository.AddAsync(Category);

                    return new OkObjectResult(new RequestResultWithData<Category>(creationResult.Item1, creationResult.Item2));
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(new RequestResultWithData<Category>(false, ex.ToString()));
                }
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();

                return new BadRequestObjectResult(new RequestResultWithData<Category>(false, JsonConvert.SerializeObject(errorList)));
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
}
