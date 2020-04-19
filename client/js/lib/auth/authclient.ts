import * as axios from "axios";
import { AuthToken, User, Password, Email, SessionInfo } from "./models";
import { Response } from "../common/models";
import { UserDto, SessionInfoDto } from "./dto";
import * as r from "../common/request";
import { ClientBase } from "../common/client";

export class AuthClient extends ClientBase {
    /**
     * Registers user with specified login and email
     * @param user User object with user info
     * @param timeout Timeout, milliseconds
     */
    public async registerAsync(
        user: User,
        timeout = 30000
    ): Promise<Response<void>> {
        const request = (): Promise<axios.AxiosResponse<void>> =>
            this.rest.post(
                "api/auth/v1/user/register",
                UserDto.fromModel(user),
                {
                    timeout: timeout,
                }
            );
        return await r.tryMakeRequestAsync(request, () => undefined);
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
        timeout = 30000
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
        return await r.tryMakeRequestAsync(request, (data) => String(data));
    }

    public async getSessionInfoAsync(
        authToken: AuthToken,
        timeout = 30000
    ): Promise<Response<SessionInfo>> {
        const request = (): Promise<axios.AxiosResponse<SessionInfoDto>> =>
            this.rest.get("api/auth/v1/sessions/my", {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + authToken,
                },
            });

        return await r.tryMakeRequestAsync(request, (data) =>
            SessionInfoDto.toModel(data)
        );
    }
}
