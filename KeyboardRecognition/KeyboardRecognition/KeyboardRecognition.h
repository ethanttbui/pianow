#pragma once
#define EXPORT_API __declspec(dllexport) 
#include <opencv2\opencv.hpp>

using namespace cv;
using namespace std;

extern "C" {
	EXPORT_API int* ProcessFrame(BYTE* inBytes, int width, int height);
	EXPORT_API void FreeMemory();
}
