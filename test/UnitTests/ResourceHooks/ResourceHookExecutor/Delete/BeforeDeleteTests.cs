﻿using JsonApiDotNetCore.Services;
using JsonApiDotNetCoreExample.Models;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.ResourceHooks
{
    public class BeforeDeleteTests : HooksTestsSetup
    {
        private readonly ResourceHook[] targetHooks = { ResourceHook.BeforeDelete };

        [Fact]
        public void BeforeDelete()
        {
            // arrange
            var discovery = SetDiscoverableHooks<TodoItem>(targetHooks, DisableDbValues);
            (var contextMock, var hookExecutor, var resourceDefinitionMock) = CreateTestObjects(discovery);

            var todoList = CreateTodoWithOwner();
            // act
            hookExecutor.BeforeDelete(todoList, ResourceAction.Delete);

            // assert
            resourceDefinitionMock.Verify(rd => rd.BeforeDelete(It.IsAny<HashSet<TodoItem>>(), It.IsAny<ResourceAction>()), Times.Once());
            resourceDefinitionMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void BeforeDelete_Without_Any_Hook_Implemented()
        {
            // arrange
            var discovery = SetDiscoverableHooks<TodoItem>(NoHooks, DisableDbValues);
            (var contextMock, var hookExecutor, var resourceDefinitionMock) = CreateTestObjects(discovery);

            var todoList = CreateTodoWithOwner();
            // act
            hookExecutor.BeforeDelete(todoList, ResourceAction.Delete);

            // assert
            resourceDefinitionMock.VerifyNoOtherCalls();
        }
    }
}

