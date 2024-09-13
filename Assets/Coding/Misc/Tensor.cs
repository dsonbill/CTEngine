using System;
using System.Collections.Generic;
using UnityEngine;

public class Tensor
{
    // The data for the tensor (stored as a multidimensional array)
    private float[][][] data;

    // The weights for the tensor (stored as a multidimensional array)
    private float[][][] weights;

    // The dimensionality relations (encoded within the data structure)
    private int[] dimensions;

    // The indicies for the tensor (stored as a multidimensional array)
    private int[][][] indicies;

    // The result tensor object
    private Tensor resultTensor;

    // Constructor
    public void Init(float[] dimension1, float[] dimension2, float[] dimension3, float[] dimension4)
    {
        // Store the dimensions
        dimensions = new int[] { dimension1.Length, dimension2.Length, dimension3.Length, dimension4.Length };

        // Initialize the data and weights arrays
        data = new float[dimensions[0]][][];
        weights = new float[dimensions[0]][][];
        for (int i = 0; i < dimensions[0]; i++)
        {
            data[i] = new float[dimensions[1]][];
            weights[i] = new float[dimensions[1]][];
            for (int j = 0; j < dimensions[1]; j++)
            {
                data[i][j] = new float[dimensions[2]];
                weights[i][j] = new float[dimensions[2]];
                for (int k = 0; k < dimensions[2]; k++)
                {
                    // Assign values to the tensor
                    data[i][j][k] = dimension1[i] * dimension2[j] * dimension3[k] * dimension4[k]; // Use all dimensions

                    // Assign values to the weights
                    weights[i][j][k] = (i + 1) * (j + 1) * (k + 1) * UnityEngine.Random.Range(0.0f, 1.0f); // Use all dimensions and randomize the weights
                }
            }
        }
    }

    // Getter for the tensor data
    public float[][][] Data
    {
        get { return data; }
    }

    // Getter for the tensor weights
    public float[][][] Weights
    { 
        get { return weights; }
    }

    // Getter for the tensor dimensions
    public int[] Dimensions
    {
        get { return dimensions; }
    }

    // Tensor Addition
    public Tensor Add(Tensor tensor)
    {
        if (Dimensions.Length != tensor.Dimensions.Length)
        {
            throw new ArgumentException("Tensors must have the same number of dimensions for addition.");
        }

        // Check if the dimensions are compatible
        for (int i = 0; i < Dimensions.Length; i++)
        {
            if (Dimensions[i] != tensor.Dimensions[i])
            {
                throw new ArgumentException("Tensors must have compatible dimensions for addition.");
            }
        }

        resultTensor = new Tensor();

        // Create a new tensor with the same dimensions
        resultTensor.Init(
            new float[Dimensions[0]],
            new float[Dimensions[1]],
            new float[Dimensions[2]],
            new float[Dimensions[3]]
        );

        // Add corresponding elements
        for (int i = 0; i < Dimensions[0]; i++)
        {
            for (int j = 0; j < Dimensions[1]; j++)
            {
                for (int k = 0; k < Dimensions[2]; k++)
                {
                    resultTensor.Data[i][j][k] = Data[i][j][k] + tensor.Data[i][j][k];
                    resultTensor.Data[i][j][k] *= tensor.Weights[i][j][k] * Weights[i][j][k];
                }
            }
        }

        return resultTensor;
    }

    // Tensor Subtraction
    public Tensor Subtract(Tensor tensor)
    {
        if (Dimensions.Length != tensor.Dimensions.Length)
        {
            throw new ArgumentException("Tensors must have the same number of dimensions for subtraction.");
        }

        // Check if the dimensions are compatible
        for (int i = 0; i < Dimensions.Length; i++)
        {
            if (Dimensions[i] != tensor.Dimensions[i])
            {
                throw new ArgumentException("Tensors must have compatible dimensions for subtraction.");
            }
        }

        resultTensor = new Tensor();

        // Create a new tensor with the same dimensions
        resultTensor.Init(
            new float[Dimensions[0]],
            new float[Dimensions[1]],
            new float[Dimensions[2]],
            new float[Dimensions[3]]
        );

        // Subtract corresponding elements
        for (int i = 0; i < Dimensions[0]; i++)
        {
            for (int j = 0; j < Dimensions[1]; j++)
            {
                for (int k = 0; k < Dimensions[2]; k++)
                {
                    resultTensor.Data[i][j][k] = Data[i][j][k] - tensor.Data[i][j][k];
                    resultTensor.Data[i][j][k] *= tensor.Weights[i][j][k] * Weights[i][j][k];
                }
            }
        }

        return resultTensor;
    }

    // Tensor Transpose
    public Tensor Transpose()
    {
        resultTensor = new Tensor();

        // Create a new tensor with dimensions reversed
        resultTensor.Init(
            new float[Dimensions[3]],
            new float[Dimensions[2]],
            new float[Dimensions[1]],
            new float[Dimensions[0]]
        );

        // Transpose the elements
        for (int i = 0; i < Dimensions[0]; i++)
        {
            for (int j = 0; j < Dimensions[1]; j++)
            {
                for (int k = 0; k < Dimensions[2]; k++)
                {
                    resultTensor.Data[k][j][i] = Data[i][j][k];
                    resultTensor.Data[k][j][i] *= Weights[i][j][k];
                }
            }
        }

        return resultTensor;
    }

    // Tensor Multiplication (Matrix Multiplication)
    public Tensor Multiply(Tensor tensor)
    {
        if (Dimensions.Length != tensor.Dimensions.Length)
        {
            throw new ArgumentException("Tensors must have the same number of dimensions for multiplication.");
        }

        // Check if the dimensions are compatible
        for (int i = 0; i < Dimensions.Length; i++)
        {
            if (Dimensions[i] != tensor.Dimensions[i])
            {
                throw new ArgumentException("Tensors must have compatible dimensions for multiplication.");
            }
        }

        resultTensor = new Tensor();

        // Create a new tensor with the same dimensions
        resultTensor.Init(
            new float[Dimensions[0]],
            new float[Dimensions[1]],
            new float[Dimensions[2]],
            new float[Dimensions[3]]
        );

        // Perform matrix multiplication
        for (int i = 0; i < Dimensions[0]; i++)
        {
            for (int j = 0; j < Dimensions[1]; j++)
            {
                for (int l = 0; l < Dimensions[3]; l++)
                {
                    for (int k = 0; k < Dimensions[2]; k++)
                    {
                        // Perform the element-wise multiplication with weights from opposite tensor
                        // Also, we can scale the weights by the dimensions of the tensors if need be
                        resultTensor.Data[i][j][k] += Data[i][j][l] * tensor.Data[l][j][k];
                        resultTensor.Data[i][j][k] *= tensor.Weights[i][j][l] * Weights[l][j][k];
                    }
                }
            }
        }

        return resultTensor;
    }

    // Method to calculate the tensor product of two tensors (Conceptual)
    public Tensor TensorProduct(Tensor tensor)
    {
        // Get the dimensions of the input tensors
        int dim1 = Data.Length;
        int dim2 = Data[0].Length;
        int dim3 = Data[0][0].Length;
        int dim4 = tensor.Data.Length;

        resultTensor = new Tensor();

        // Create the result tensor with the combined dimensions
        resultTensor.Init(
            new float[dim1 * (dim4 + 1)],
            new float[dim2],
            new float[dim3],
            new float[dim4]
        );

        // Calculate the tensor product
        for (int i = 0; i < dim1; i++)
        {
            for (int j = 0; j < dim2; j++)
            {
                for (int k = 0; k < dim3; k++)
                {
                    for (int l = 0; l < dim4; l++)
                    {
                        // Perform the element-wise multiplication and sum 
                        resultTensor.Data[i * dim4 + l][j][k] += Data[i][j][k] * tensor.Data[l][j][k];
                        resultTensor.Data[i * dim4 + l][j][k] *= tensor.Weights[i][j][k] * Weights[l][j][k];
                    }
                }
            }
        }

        return resultTensor;
    }
}