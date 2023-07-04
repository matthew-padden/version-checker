﻿using Microsoft.AspNetCore.Mvc;
using VersionChecker.Api.Model;
using VersionChecker.Infrastructure;

namespace VersionChecker.Api.Controller
{
    public abstract class VersionCheckerController<T> : ControllerBase
        where T : IVersionDetail
    {
        protected readonly IRepository<T> repository;

        protected VersionCheckerController(IRepository<T> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        protected async Task<IActionResult> GetVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
                return BadRequest();

            var versions = await repository.GetAsync();
            var match = versions.FirstOrDefault(v => v.Version == version);

            if (match is null)
                return BadRequest();

            return Ok(match);
        }

        protected async Task<IActionResult> GetByAdditionalProperty(string property, string value)
        {
            if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(value))
                return Ok(await repository.GetAsync());

            var dotNetVersion = await repository.GetByAdditionalPropertyAsync(property, value);
            if (dotNetVersion == null)
                return BadRequest();

            return Ok(dotNetVersion);
        }
    }
}
