import * as axios from "axios";
import { AuthToken, User, Password, Email, SessionInfo } from "./models";
import { Response } from "../common/models";
import { UserDto, SessionInfoDto } from "./dto";

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
     * @param user User object with user info
     * @param timeout Timeout, milliseconds
     */
    public async registerAsync(
        user: User,
        timeout = 2500
    ): Promise<Response<void>> {
        const request = (): Promise<axios.AxiosResponse<void>> =>
            this.rest.post(
                "api/auth/v1/user/register",
                UserDto.fromModel(user),
                {
                    timeout: timeout,
                }
            );
        return await this.tryMakeRequestAsync(request, () => undefined);
    }

    /**
     * Attempts to authenticate user with provided credentials
     * @param email User email
     * @param password User password
     * @param timeout Timeout, milliseconds
     * @returns Authentication token
     */
    public async authenticateAsync(
        email: Email,
        password: Password,
        timeout = 2500
    ): Promise<Response<AuthToken>> {
        const request = (): Promise<axios.AxiosResponse<string>> =>
            this.rest.post(
                "api/auth/v1/user/authenticate",
                {
                    email: email,
                    password: password,
                },
                {
                    timeout: timeout,
                }
            );
        return await this.tryMakeRequestAsync(request, (data) => String(data));
    }

    public async getSessionInfoAsync(
        authToken: AuthToken,
        timeout = 2500
    ): Promise<Response<SessionInfo>> {
        const request = (): Promise<axios.AxiosResponse<SessionInfoDto>> =>
            this.rest.get("api/auth/v1/sessions/my", {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + authToken,
                },
            });

        return await this.tryMakeRequestAsync(request, (data) =>
            SessionInfoDto.toModel(data)
        );
    }

    private async tryMakeRequestAsync<TDto, TResult>(
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
            const errorInfo = String(error.response.data);
            const statusCode: number = error.response.status;

            return new Response<TResult>(statusCode, undefined, errorInfo);
        }
    }
}
