using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.UI {
	public enum Operation {
		Find, //This can find processes or lockers depending on --type=process or --type=blockers
		Kill,
		Performance,
		Read, //These will be mainly used by custom commands that will be called via "scripts"
		Write,
		Custom, //Custom command that executes a series of HM cmds in the form of a script
		Help,
	}

}
