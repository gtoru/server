import * as nock from "nock";
import { TaskClient } from "./taskclient";
import {
    GetTaskResponse,
    GetAllTasksResponse,
    CreateTaskResponse,
} from "./dto";
import { Task } from "./models";

const baseUrl = "http://localhost";
const token = "1234";
const taskId = "987-654";
const client = new TaskClient(baseUrl);

afterEach(() => {
    nock.restore();
    nock.activate();
});

describe("task client", () => {
    it("gets single task", async () => {
        const response: GetTaskResponse = {
            taskId: "",
            answer: "42",
            question: "Meaning of life",
            variants: ["42", "Love", "Death"],
        };

        const scope = nock(baseUrl)
            .get(`/api/v1/task/${taskId}`)
            .matchHeader("Authorization", "Bearer " + token)
            .reply(200, JSON.stringify(response));

        const taskResponse = await client.getTaskAsync(taskId, token);

        expect(taskResponse.responseCode).toBe(200);
        expect(taskResponse.errorInfo).toBeUndefined();
        expect(taskResponse.responseData.asnwer).toBe(response.answer);
        expect(taskResponse.responseData.question).toBe(response.question);
        expect(taskResponse.responseData.variants).toStrictEqual(
            response.variants
        );

        scope.done();
    });

    it("gets all tasks", async () => {
        const taskResponse: GetTaskResponse = {
            answer: "42",
            question: "Meaning of life",
            taskId: "9234-1234",
            variants: ["Life", "Death", "42"],
        };
        const response: GetAllTasksResponse = {
            tasks: [taskResponse],
        };

        const scope = nock(baseUrl)
            .get("/api/v1/task/all")
            .matchHeader("Authorization", "Bearer " + token)
            .reply(200, JSON.stringify(response));

        const allTasksResponse = await client.getAllTasksAsync(token);

        expect(allTasksResponse.responseCode).toBe(200);
        expect(allTasksResponse.errorInfo).toBeUndefined();
        expect(allTasksResponse.responseData).toStrictEqual(
            GetAllTasksResponse.toModel(response)
        );

        scope.done();
    });

    it("creates new task", async () => {
        const task: Task = {
            asnwer: "42",
            question: "Meaning of life",
            taskId: "123",
            variants: ["Love", "Death", "42"],
        };

        const response: CreateTaskResponse = {
            taskId: taskId,
        };

        const scope = nock(baseUrl)
            .post("/api/v1/task/new")
            .matchHeader("Authorization", "Bearer " + token)
            .reply(200, JSON.stringify(response));

        const taskResponse = await client.createTaskAsync(task, token);

        expect(taskResponse.responseCode).toBe(200);
        expect(taskResponse.errorInfo).toBeUndefined();
        expect(taskResponse.responseData).toBe(taskId);

        scope.done();
    });
});
