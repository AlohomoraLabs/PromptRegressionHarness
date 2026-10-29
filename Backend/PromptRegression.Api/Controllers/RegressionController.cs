using Microsoft.AspNetCore.Mvc;
using PromptRegression.Api.Models;
using PromptRegression.Api.Services;

namespace PromptRegression.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RegressionController(RegressionService regressionService) : ControllerBase
    {
        [HttpPost("run")]
        public async Task<ActionResult<EvaluationResult>> Run(RegressionRequest request, CancellationToken cancellationToken)
        {
            var result = await regressionService.RunAsync(
                request,
                cancellationToken);

            return Ok(result);
        }


        [HttpPost("run-batch")]
        public async Task<ActionResult<BatchRegressionResult>> RunBatch(
        BatchRegressionRequest request,
        CancellationToken cancellationToken)
        {
            var result = await regressionService.RunBatchAsync(
                request.Prompt,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("compare")]
        public async Task<ActionResult<PromptComparisonResult>> Compare(
    PromptComparisonRequest request,
    CancellationToken cancellationToken)
        {
            var result = await regressionService.CompareAsync(
                request.PromptV1,
                request.PromptV2,
                cancellationToken);

            return Ok(result);
        }
    }
}