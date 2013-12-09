﻿// Copyright © 2013 Open Octopus Ltd.
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
// 
// Website: http://www.opencbs.com
// Contact: contact@opencbs.com

using NSubstitute;
using NUnit.Framework;
using OpenCBS.DataContract;
using OpenCBS.DataContract.AppEvent;
using OpenCBS.DataContract.CommandData;
using OpenCBS.Interface;
using OpenCBS.Interface.Presenter;
using OpenCBS.Interface.Service;
using OpenCBS.Service.Command;

// ReSharper disable InconsistentNaming
namespace OpenCBS.UnitTest.Command
{
    [TestFixture]
    public class EditRoleCommandTest
    {
        private IRolePresenter _presenter;
        private IRoleService _roleService;
        private IApplicationController _appController;
        private EditRoleCommand _command;

        [SetUp]
        public void SetUp()
        {
            _presenter = Substitute.For<IRolePresenter>();
            _roleService = Substitute.For<IRoleService>();
            _appController = Substitute.For<IApplicationController>();
            _command = new EditRoleCommand(_presenter, _roleService, _appController);
        }

        [Test]
        public void Execute_CallsFindByIdOnRoleService()
        {
            _presenter.Get(Arg.Any<RoleDto>()).Returns(new Result<RoleDto>(CommandResult.Cancel, null));
            _command.Execute(new EditRoleData { Id = 1 });
            _roleService.Received().FindById(Arg.Is<int>(id => id == 1));
        }

        [Test]
        public void Execute_CommandResultIsNotOk_DoesNotSaveRole()
        {
            _presenter.Get(Arg.Any<RoleDto>()).Returns(new Result<RoleDto>(CommandResult.Cancel, null));
            _command.Execute(new EditRoleData { Id = 1 });
            _roleService.DidNotReceive().Update(Arg.Any<RoleDto>());
            _appController.DidNotReceive().Raise(Arg.Any<RoleSavedEvent>());
        }

        [Test]
        public void Execute_CommandResultIsOk_SavesRole()
        {
            var roleDto = new RoleDto { Id = 1 };
            _roleService.FindById(Arg.Is<int>(id => id == 1)).Returns(roleDto);
            _presenter.Get(Arg.Is<RoleDto>(dto => dto.Id == 1)).Returns(new Result<RoleDto>(CommandResult.Ok, roleDto));
            _command.Execute(new EditRoleData { Id = 1 });
            _roleService.Received().Update(Arg.Is<RoleDto>(dto => dto.Id == 1));
            _appController.Received().Raise(Arg.Is<RoleSavedEvent>(data => data.Id == 1));
        }
    }
}
// ReSharper restore InconsistentNaming
