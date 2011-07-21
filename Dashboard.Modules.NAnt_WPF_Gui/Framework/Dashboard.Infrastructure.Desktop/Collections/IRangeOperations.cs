#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of Daniel Vaughan's base library

	Daniel Vaughan's base library is free software: you can redistribute it and/or modify
	it under the terms of the GNU Lesser General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Daniel Vaughan's base library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with DanielVaughan's base library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2011-01-10 16:27:11Z</CreationDate>
</File>
*/
#endregion

using System.Collections;

namespace Techno_Fly.Tools.Dashboard.Collections
{
	/* TODO: [DV] Comment. */
	public interface IRangeOperations
	{
		void AddRange(IEnumerable items);
		void RemoveRange(IEnumerable items);
	}
}
