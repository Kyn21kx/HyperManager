using HyperManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperManager.DTOs {
	public interface IParseable {

		IParseable SetFromCommand(CommandBuilder command);

	}
}
