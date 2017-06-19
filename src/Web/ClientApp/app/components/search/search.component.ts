import { Component } from '@angular/core';

@Component({
    selector: 'search',
    templateUrl: './search.component.html'
})
export class SearchComponent {
    public url: string = 'http://www.example.com';
    public sseUrl: string = 'api/SampleData/Sse';

    find(event: Event) {
        
        let source = new EventSource(this.sseUrl);

        source.onmessage = function (event) {
            console.log('onmessage: ' + event.data);
        };
        
    }

}
