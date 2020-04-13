import {
    CreateTaskRequest,
    CreateTaskResponse,
    GetTaskResponse,
    GetAllTasksResponse,
} from "./dto";
import { Task, TaskId } from "./models";
import { ClientBase } from "../common/client";
import { Response } from "../common/models";
import * as r from "../common/request";
import { AuthToken } from "../auth/models";
import * as axios from "axios";

export class TaskClient extends ClientBase {
    /**
     * Creates new task
     * @param task task model. ID is not used in request
     * @param token authentication token
     * @param timeout timeout milliseconds
     */
    public async createTaskAsync(
        task: Task,
        token: AuthToken,
        timeout?: number
    ): Promise<Response<TaskId>> {
        const request = (): Promise<
            axios.AxiosResponse<CreateTaskResponse>
        > => {
            return this.rest.post(
                "api/v1/task/new",
                CreateTaskRequest.fromModel(task),
                {
                    timeout: timeout,
                    headers: {
                        Authorization: "Bearer " + token,
                    },
                }
            );
        };

        return await r.tryMakeRequestAsync(request, (data) => data.taskId);
    }

    /**
     * Gets single task
     * @param taskId ID of the task
     * @param token Authentication token
     * @param timeout Request timeout milliseconds
     */
    public async getTaskAsync(
        taskId: TaskId,
        token: AuthToken,
        timeout?: number
    ): Promise<Response<Task>> {
        const request = (): Promise<axios.AxiosResponse<GetTaskResponse>> =>
            this.rest.get(`api/v1/task/${taskId}`, {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });
        return await r.tryMakeRequestAsync(request, (data) =>
            GetTaskResponse.toModel(data)
        );
    }

    /**
     * Gets all available tasks
     * @param token Authentication token
     * @param timeout Request timeout milliseconds
     */
    public async getAllTasksAsync(
        token: AuthToken,
        timeout?: number
    ): Promise<Response<Task[]>> {
        const request = (): Promise<axios.AxiosResponse<GetAllTasksResponse>> =>
            this.rest.get("api/v1/task/all", {
                timeout: timeout,
                headers: {
                    Authorization: "Bearer " + token,
                },
            });

        return await r.tryMakeRequestAsync(request, (data) =>
            GetAllTasksResponse.toModel(data)
        );
    }
}
