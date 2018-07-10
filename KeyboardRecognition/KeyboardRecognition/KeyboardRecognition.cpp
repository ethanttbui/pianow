#include "pch.h"
#include "KeyboardRecognition.h"

extern "C" {
	int* result;

	void FreeMemory()
	{
		if (result != NULL) {
			delete result;
		}
	}

	int* ProcessFrame(BYTE* inBytes, int width, int height)
	{
		result = NULL;

		// Read the bytes to a Mat
		Mat inMat = Mat(height, width, CV_8UC4, inBytes);

		// Convert to binary image with threshold 200
		Mat outMat;
		cvtColor(inMat, outMat, CV_BGRA2GRAY);
		threshold(outMat, outMat, 200, 255, 0);

		// Perform morphological closing operation to remove edges between keys
		Mat kernel = getStructuringElement(1, Size(5, 5), Point(-1, -1));
		morphologyEx(outMat, outMat, CV_MOP_CLOSE, kernel);

		// Find contours
		vector<vector<Point>> contours;
		vector<Vec4i> hierarchy;
		findContours(outMat, contours, hierarchy, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));

		// First, filter the contours based on the dimensions and size of min-area rectangles
		// Next, only the contours that can be approximated to a quadrangle will be reserved
		// Their gravity centers and eigen vectors are calculated
		for (unsigned int i = 0; i < contours.size(); i++)
		{
			RotatedRect rotatedRect = minAreaRect(contours[i]);
			double w = rotatedRect.size.width;
			double h = rotatedRect.size.height;
			if ((h >= 5 * w && h >= 0.5 * height) || (w >= 5 * h && w >= 0.5 * width))
			{
				// Obtain the convex hull of the contour
				vector<Point> hull;
				convexHull(contours[i], hull);

				// Further approximate the convex hull using Douglas-Peucker method
				double epsilon = 0.015 * arcLength(hull, true);
				vector<Point> approxPoly;
				approxPolyDP(hull, approxPoly, epsilon, true);

				if (approxPoly.size() == 4)
				{
					result = new int[4];

					// Find the gravity center
					Moments mo = moments(hull);
					result[0] = int(mo.m10 / mo.m00);
					result[1] = int(mo.m01 / mo.m00);

					// Construct a buffer used by the PCA analysis
					Mat dataPts = Mat(hull.size(), 2, CV_64FC1);
					for (int i = 0; i < dataPts.rows; ++i)
					{
						dataPts.at<double>(i, 0) = hull[i].x;
						dataPts.at<double>(i, 1) = hull[i].y;
					}

					// Perform PCA analysis
					PCA pca_analysis(dataPts, Mat(), CV_PCA_DATA_AS_ROW);

					// Obtain the eigen vector coordinate
					double* eigenVec = new double[2];
					eigenVec[0] = pca_analysis.eigenvectors.at<double>(0, 0);
					eigenVec[1] = pca_analysis.eigenvectors.at<double>(0, 1);

					// Express the eigen vector using another point 
					// with an appropriate offset from the gravity center
					result[2] = int(result[0] + 100 * eigenVec[0]);
					result[3] = int(result[1] + 100 * eigenVec[1]);

					break;
				}
			}
		}

		return result;
	}
}