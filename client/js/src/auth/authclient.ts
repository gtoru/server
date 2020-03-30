import * as axios from "axios";
import { AuthToken } from "./models";
import { Response } from "../models";

export class AuthClient {
    private rest: axios.AxiosInstance;

    /**
     * Creates client to preform authentication tasks
     * @param serverUrl Base URL of GTO-RU server
     */
    public constructor(serverUrl: string) {
        this.rest = axios.default.create({
            baseURL: serverUrl,
        });
    }

    /**
     * Registers user with specified login and email
     * @param login User email
     * @param password User password
     * @param timeout Timeout, milliseconds
     */
    public async registerAsync(
        login: string,
        password: string,
        timeout = 2500
    ): Promise<Response<AuthToken>> {
        try {
            const response = await this.rest.post(
                `/auth/v1/register?email=${login}`,
                password,
                {
                    timeout: timeout,
                }
            );

            return new Response<AuthToken>(
                response.status,
                String(response.data)
            );
        } catch (err) {
            const error: axios.AxiosError = err;
            const errorInfo = String(error.response.data);
            const statusCode: number = error.response.status;

            return new Response<AuthToken>(statusCode, undefined, errorInfo);
        }
    }
}
