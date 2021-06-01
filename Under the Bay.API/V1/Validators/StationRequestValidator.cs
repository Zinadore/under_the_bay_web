using System;
using FluentValidation;
using NodaTime;
using Under_the_Bay.API.V1.Contracts.Requests;

namespace Under_the_Bay.API.V1.Validators
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
                .LessThanOrEqualTo(DateTimeOffset.Now)
                .GreaterThan(x => x.StartDate)
                .When(x => x.IncludeMeasurements && x.StartDate.HasValue);
        }
    }
}