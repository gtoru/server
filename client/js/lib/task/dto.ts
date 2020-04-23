import { Task } from "./models";

export class CreateTaskRequest {
    public weight: number;
    public question: string;
    public answer: string;
    public variants: string[];

    public static fromModel(task: Task): CreateTaskRequest {
        return {
            answer: task.answer,
            question: task.question,
            variants: task.variants,
            weight: task.weight,
        };
    }
}

export class CreateTaskResponse {
    public taskId: string;
}

export class GetTaskResponse {
    public weight: number;
    public taskId: string;
    public question: string;
    public answer: string;
    public variants: string[];
    public static toModel(task: GetTaskResponse): Task {
        return {
            answer: task.answer,
            weight: task.weight,
            question: task.question,
            taskId: task.taskId,
            variants: task.variants,
        };
    }
}

export class GetAllTasksResponse {
    public tasks: GetTaskResponse[];
    public static toModel(tasks: GetAllTasksResponse): Task[] {
        return tasks.tasks.map((t) => GetTaskResponse.toModel(t));
    }
}
