// JAAsm.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "JAAsm.h"


// This is an example of an exported variable
JAASM_API int nJAAsm=0;

// This is an example of an exported function.
JAASM_API int fnJAAsm(void)
{
    return 0;
}

// This is the constructor of a class that has been exported.
CJAAsm::CJAAsm()
{
    return;
}
