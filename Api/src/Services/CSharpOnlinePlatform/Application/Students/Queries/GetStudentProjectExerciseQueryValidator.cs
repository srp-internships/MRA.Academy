﻿using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Students.Queries
{
    public class GetStudentProjectExerciseQueryValidator : AbstractValidator<GetStudentProjectExerciseQuery>
    {
        public GetStudentProjectExerciseQueryValidator(IApplicationDbContext dbContext)
        {
            RuleFor(query => query.ThemeId).Must(themeGuid =>
            {
                return dbContext.GetEntities<Theme>().AsNoTracking().Any(t => t.Id == themeGuid);
            }).WithMessage($"Тема не найдена.");
            
        }
    }
}
