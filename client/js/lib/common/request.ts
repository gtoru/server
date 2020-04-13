import * as axios from "axios";
import { Response } from "./models";

export async function tryMakeRequestAsync<TDto, TResult>(
    request: () => Promise<axios.AxiosResponse<TDto>>,
    responseExtractor: (data: TDto) => TResult
): Promise<Response<TResult>> {
    try {
        const response = await request();
        return new Response<TResult>(
            response.status,
            responseExtractor(response.data)
        );
    } catch (err) {
        const error: axios.AxiosError = err;
        const errorInfo = String(error.response?.data);
        const statusCode: number = error.response?.status;

        return new Response<TResult>(statusCode, undefined, errorInfo);
    }
}
