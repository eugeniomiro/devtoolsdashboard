#region File and License Information
/*
<File>
	<Copyright>Copyright © 2010, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY <copyright holder> ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-11-26 11:56:06Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Techno_Fly.Tools.Dashboard
{
    public class EnvironmentValues
    {
        static bool? designTime;

        public static bool DesignTime
        {
            get
            {
                if (!designTime.HasValue)
                {
#if SILVERLIGHT
					designTime = DesignerProperties.IsInDesignTool;
#else
                    designTime = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(
                        typeof(System.Windows.DependencyObject)).DefaultValue;
#endif
                }

                return designTime.Value;
            }
        }

#if WINDOWS_PHONE
		static bool? usingEmulator;

		public static bool UsingEmulator
		{
			get
			{
				if (!usingEmulator.HasValue)
				{
					usingEmulator = Microsoft.Devices.Environment.DeviceType 
						== Microsoft.Devices.DeviceType.Emulator;
				}
				return usingEmulator.Value;
			}
		}

		static bool? usingDevice;

		public static bool UsingDevice
		{
			get
			{
				if (!usingDevice.HasValue)
				{
					usingDevice = Microsoft.Devices.Environment.DeviceType 
						== Microsoft.Devices.DeviceType.Device;
				}
				return usingDevice.Value;
			}
		}
#endif
    }
}
