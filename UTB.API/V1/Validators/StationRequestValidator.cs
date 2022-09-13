using System;
using FluentValidation;
using NodaTime;
using UTB.API.V1.Contracts.Requests;

namespace UTB.API.V1.Validators
{
    public class StationRequestValidator: AbstractValidator<StationRequest>
    {
        public StationRequestValidator()
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .LessThanOrEqualTo(DateTimeOffset.Now)
                .When(x => x.IncludeMeasurements);
            
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.IncludeMeasurements && x.StartDate.HasValue);
        }
    }
}