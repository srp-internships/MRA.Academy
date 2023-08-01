﻿using Application.Students.Queries;
using Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using static Application.IntegrationTest.TestHelper;
using Core.Exceptions;

namespace Application.IntegrationTest.Themes
{
    public class GetThemeQueryTest
    {
        [Test]
        public async Task GetTheme_ShouldReturnThemeOfCourseFromDataBaseTest()
        {
            await RunAsStudentAsync();

            var courseGuid = Guid.NewGuid();
            var course = CreateTestCourse(courseGuid);

            var theme = CreateTheme(course, DateTime.Now.AddDays(-1));

            await AddAsync(theme);

            GetThemeQuery query = new(theme.Id, GetAuthenticatedUserId());

            var themeDto = await SendAsync(query);
            themeDto.Id.Should().Be(theme.Id);
        }

        [Test]
        public async Task GetThemeWithStartDate_ShouldReturnThemeOfCourseFromDataBaseTest()
        {
            await RunAsStudentAsync();

            var courseGuid = Guid.NewGuid();
            var course = CreateTestCourse(courseGuid);

            var theme = CreateTheme(course, DateTime.Now);

            await AddAsync(theme);

            GetThemeQuery query = new(theme.Id, GetAuthenticatedUserId());

            var themeDto = await SendAsync(query);
            themeDto.Id.Should().Be(theme.Id);
        }

        [Test]
        public async Task GetThemeWithNotstarted_ShouldReturnThemeOfCourseFromDataBaseTest()
        {
            await RunAsStudentAsync();

            var courseGuid = Guid.NewGuid();
            var course = CreateTestCourse(courseGuid);

            var theme = CreateTheme(course, DateTime.Now.AddDays(23));

            await AddAsync(theme);

            GetThemeQuery query = new(theme.Id, GetAuthenticatedUserId());

            ValidationFailureException validationError = Assert.ThrowsAsync<ValidationFailureException>(() => SendAsync(query));
            var firstNameMustNotBeEmptyErrorWasShown = IsErrorExists("ThemeGuid", "Это тема ешё не началас.", validationError);
        }

        [Test]
        public async Task GetThemeQuery_NotExistingStudentId_NotFoundException()
        {
            await RunAsTeacherAsync();
            var teacher = await GetAuthenticatedUser<Teacher>();
            var course = new Course() { Id = Guid.NewGuid(), LearningLanguage = "Tajik", Name = "Name", TeacherId = teacher.Id };
            await AddAsync(course);
            var theme = new Theme() { Id = Guid.NewGuid(), Content = "Content", CourseId = course.Id, Name = "Name"};
            await AddAsync(theme);
            var command = new GetThemeQuery(theme.Id, Guid.NewGuid());

            ValidationFailureException validationFailureException = Assert.ThrowsAsync<ValidationFailureException>(() => SendAsync(command));
            var notFoundExceptionShown = IsErrorExists("StudentGuid", "Студент не найден.", validationFailureException);
        }

        #region Test Data

        Course CreateTestCourse(Guid guid)
        {
            return new Course()
            {
                Id = guid,
                Name = "C# Training",
                LearningLanguage = "Tajik",
            };
        }

        Theme CreateTheme(Course course, DateTime startDate)
        {
            return new Theme()
            {
                Id = Guid.NewGuid(),
                Name = $"Chapter 1",
                Content = "Test",
                Course = course,
                StartDate = startDate,
                Exercises = CreateExercise()
            };
        }

        List<Exercise> CreateExercise()
        {
            var exercises = new List<Exercise>()
            {
                new Exercise()
                {
                    Id = Guid.NewGuid(),
                    Name = "Integer",
                    Description = "A distanceLis given in centimeters. Find the amount of full meters of this distance (1m=1000cm). Use the operator of integer division. ",
                    Template = "Template",
                    Test = "Test"
                },
                new Exercise()
                {
                    Id = Guid.NewGuid(),
                    Name = "String",
                    Description = "A string is an object of type String whose value is text. Internally, the text is stored as a sequential read-only collection of Char objects.",
                    Template = "Template",
                    Test = "Test"
                },
                new Exercise()
                {
                    Id = Guid.NewGuid(),
                    Name = "double",
                    Description = "There is only one implicit conversion between floating-point numeric types: from float to double.",
                    Template = "Template",
                    Test = "Test"
                }
            };
            return exercises;
        }
        #endregion
    }
}
