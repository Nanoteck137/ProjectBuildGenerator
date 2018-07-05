using System.Collections.Generic;
using System;

namespace Config {

class Target
{
    public string name = null;
    public string output = null;
    public string type = null;
}

class Options
{
    public string compiler = null;
    public string packer = null;
}

class Data
{
    public Options windows = null;
    public Options linux = null;
    public List<Target> targets = null;
}

}