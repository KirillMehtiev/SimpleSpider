import { Component } from '@angular/core';

@Component({
    selector: 'search',
    templateUrl: './search.component.html'
})
export class SearchComponent {
    public url: string = 'http://www.collective.com';
    public sseUrl: string = 'api/SampleData/Sse?url=';

    find(event: Event) {
        let source = new EventSource(this.sseUrl + this.url);

        source.onmessage = function (event) {
            console.log('onmessage: ' + event.data);
        };

        source.onerror = function (e) {
            source.close()
            console.log("EventSource failed.");
        };
    }

}
