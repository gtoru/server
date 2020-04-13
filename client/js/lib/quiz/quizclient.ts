import { ClientBase } from "../common/client";
import { TaskId } from "../task/models";
import { AuthToken } from "../auth/models";
import { Response } from "../common/models";
import { QuizInfo, QuizId, Quiz } from "./models";
import { AxiosResponse } from "axios";
import {
    QuizInfoDto,
    CreateQuizRequest,
    GetQuizResponse,
    GetAllQuizzesResponse,
} from "./dto";
import * as r from "../common/request";

export class QuizClient extends ClientBase {
    /**
     * Creates new quiz with provided tasks
     * @param tasks List of tasks to be used in quiz
     * @param token Authentication token
     * @param timeout Request timeout, milliseconds
     */
    public async createQuizAsync(
        tasks: TaskId[],
        token: AuthToken,
        timeout?: number
    ): Promise<Response<QuizInfo>> {
        const requestData: CreateQuizRequest = {
            tasks: tasks,
        };
        const request = (): Promise<AxiosResponse<QuizInfoDto>> =>
            this.rest.post("api/v1/quiz/new", requestData, {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });

        return await r.tryMakeRequestAsync(request, (data) =>
            QuizInfoDto.toInfo(data)
        );
    }

    /**
     * Gets quiz with provided ID
     * @param quizId Quiz id
     * @param token Authenticatino token
     * @param timeout Request timeout, milliseconds
     */
    public async getQuizAsync(
        quizId: QuizId,
        token: AuthToken,
        timeout?: number
    ): Promise<Response<Quiz>> {
        const request = (): Promise<AxiosResponse<GetQuizResponse>> =>
            this.rest.get(`api/v1/quiz/${quizId}`, {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });

        return await r.tryMakeRequestAsync(request, (data) =>
            GetQuizResponse.toModel(data)
        );
    }

    /**
     * Gets all available quizzes
     * @param token Authentication token
     * @param timeout Request timeout, milliseconds
     */
    public async getAllQuizzesAsync(
        token: AuthToken,
        timeout?: number
    ): Promise<Response<QuizInfo[]>> {
        const request = (): Promise<AxiosResponse<GetAllQuizzesResponse>> =>
            this.rest.get("api/v1/quiz/all", {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });

        return await r.tryMakeRequestAsync(request, (data) =>
            data.quizzes.map((q) => QuizInfoDto.toInfo(q))
        );
    }
}
