﻿using Application.Common.Interfaces;
using Core.ValidationsBehaviours;
using Domain.Entities;
using FluentValidation;

namespace Application.Admin.Commands.StudentCommand
{
    public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator(IApplicationDbContext dbContext)
        {
            RuleFor(studentCommand => studentCommand.LastName).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.FirstName).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.Occupation).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.PhoneNumber).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.BirthDate).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));

            RuleFor(studentCommand => studentCommand.City).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.Country).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.Region).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.Address).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));

            RuleFor(studentCommand => studentCommand.Password).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));

            RuleFor(studentCommand => studentCommand.Email).NotEmpty().WithMessage(ValidationMessages.GetNotEmptyMessage("{PropertyName}"));
            RuleFor(studentCommand => studentCommand.Email).EmailAddress().WithMessage(ValidationMessages.GetEmailAddressMessage());            
            RuleFor(studentCommand => studentCommand.Email).
                Must(email => !dbContext.GetEntities<Student>().Any(s => s.Email == email)).WithMessage("Электронная почта уже зарегистрирована. Пожалуйста, укажите другой адрес электронной почты.");

            RuleFor(studentCommand => studentCommand.CourseName).
                Must(courseName => dbContext.GetEntities<Course>().
                Any(s => s.Name.Equals(courseName))).WithMessage("Данный курс еще не зарегистрирован. Выберите существующий курс.");
        }
    }
}