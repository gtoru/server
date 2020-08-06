import { ClientBase } from "../common/client";
import {
    UserStats,
    TestSessionInfo,
    TestSessionResult,
    TaskAnswer,
} from "./models";
import { QuizId } from "../quiz/models";
import { AuthToken, UserId } from "../auth/models";
import * as axios from "axios";
import {
    TestSessionInfoResponse,
    NewSessionRequest,
    TestSessionResultsResponse,
    TestSessionResultResponse,
    AddAnswersRequest,
    AnswerDto,
    UserCountResponse,
    UserStatsResponse,
    UserStatsDto,
} from "./dto";
import * as r from "../common/request";
import { Response } from "../common/models";

export class UserClient extends ClientBase {
    /**
     * Starts new testing session for specified user, with specified quiz
     * @param userId Id of user to start session
     * @param quizId Id of quiz to be used in testSession
     * @param token User authentication token
     * @param timeout Request timeout, ms
     */
    public async startNewSessionAsync(
        userId: UserId,
        quizId: QuizId,
        token: AuthToken,
        timeout = 30000
    ): Promise<Response<TestSessionInfo>> {
        const request = (): Promise<
            axios.AxiosResponse<TestSessionInfoResponse>
        > => {
            const requestObject: NewSessionRequest = {
                quizId: quizId,
            };
            return this.rest.post(
                `api/v1/user/${userId}/sessions/new`,
                requestObject,
                {
                    timeout: timeout,
                    headers: {
                        Authorization: "Bearer " + token,
                    },
                }
            );
        };

        return await r.tryMakeRequestAsync(request, (data) =>
            TestSessionInfoResponse.toModel(data)
        );
    }

    /**
     * Gets info on active test session if any
     * @param userId Id of user whose session is returned
     * @param token Authentication token of the user, whose id is used
     * @param timeout Request timeout, ms
     */
    public async getCurrentTestSessionAsync(
        userId: UserId,
        token: AuthToken,
        timeout = 30000
    ): Promise<Response<TestSessionInfo>> {
        const request = (): Promise<
            axios.AxiosResponse<TestSessionInfoResponse>
        > => {
            return this.rest.get(`api/v1/user/${userId}/sessions/current`, {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });
        };

        return await r.tryMakeRequestAsync(request, (data) =>
            TestSessionInfoResponse.toModel(data)
        );
    }

    /**
     * Get all testing result for specified user
     * @param userId Id of user whose results need to be returned
     * @param token Authentication token of the user
     * @param timeout Request timeout, ms
     */
    public async getResultsAsync(
        userId: UserId,
        token: AuthToken,
        timeout = 30000
    ): Promise<Response<TestSessionResult[]>> {
        const request = (): Promise<
            axios.AxiosResponse<TestSessionResultsResponse>
        > => {
            return this.rest.get(`api/v1/user/${userId}/sessions/results`, {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });
        };

        return await r.tryMakeRequestAsync(request, (data) =>
            TestSessionResultsResponse.toModel(data)
        );
    }

    /**
     * End current session of specified user
     * @param userId ID of users whose session should be ended
     * @param token Authnetication token of user
     * @param timeout Request timeout, ms
     */
    public async endSessionAsync(
        userId: UserId,
        token: AuthToken,
        timeout = 30000
    ): Promise<Response<TestSessionResult>> {
        const request = (): Promise<
            axios.AxiosResponse<TestSessionResultResponse>
        > => {
            return this.rest.post(
                `api/v1/user/${userId}/sessions/end`,
                {},
                {
                    timeout: timeout,
                    headers: {
                        Authorization: "Bearer " + token,
                    },
                }
            );
        };

        return await r.tryMakeRequestAsync(request, (data) =>
            TestSessionResultResponse.toModel(data)
        );
    }

    /**
     * Adds answer to user's active test session, if any
     * @param userId Id of user who is answering
     * @param taskNumber Number of task in quiz
     * @param guess Guessed answer
     * @param token User authentication token
     * @param timeout Request timeout, ms
     */
    public async addAnswerAsync(
        userId: UserId,
        answers: TaskAnswer[],
        token: AuthToken,
        timeout = 30000
    ): Promise<Response<void>> {
        const request = (): Promise<axios.AxiosResponse<void>> => {
            const requestObject: AddAnswersRequest = {
                answers: answers.map((answer) => {
                    const answerDto: AnswerDto = new AnswerDto();
                    answerDto.answer = answer.answer;
                    answerDto.taskNumber = answer.taskNumber;
                    return answerDto;
                }),
            };

            return this.rest.post(
                `api/v1/user/${userId}/sessions/answer`,
                requestObject,
                {
                    timeout: timeout,
                    headers: {
                        Authorization: "Bearer " + token,
                    },
                }
            );
        };

        return await r.tryMakeRequestAsync(request, () => undefined);
    }

    public async getUserCountAsync(
        token: AuthToken,
        timeout = 30000
    ): Promise<Response<number>> {
        const request = (): Promise<axios.AxiosResponse<UserCountResponse>> => {
            return this.rest.get("api/v1/user/count", {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });
        };

        return await r.tryMakeRequestAsync(request, (r) => r.userCount);
    }

    public async getUserStatsAsync(
        token: AuthToken,
        timeout = 30000
    ): Promise<Response<UserStats[]>> {
        const request = (): Promise<axios.AxiosResponse<UserStatsResponse>> => {
            return this.rest.get("api/v1/user/stats", {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });
        };

        return await r.tryMakeRequestAsync(request, (r) =>
            r.userStats.map((x) => UserStatsDto.toModel(x))
        );
    }
}
