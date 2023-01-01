// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the JAASM_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// JAASM_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef JAASM_EXPORTS
#define JAASM_API __declspec(dllexport)
#else
#define JAASM_API __declspec(dllimport)
#endif

// This class is exported from the dll
class JAASM_API CJAAsm {
public:
	CJAAsm(void);
	// TODO: add your methods here.
};

extern JAASM_API int nJAAsm;

JAASM_API int fnJAAsm(void);
